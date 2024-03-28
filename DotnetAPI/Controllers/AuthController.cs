using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Interfaces;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    private readonly ReusableSql _reusableSql;
    private readonly IMapper _mapper;
    
    public AuthController(IConfiguration config, AuthHelper authHelper)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
        _reusableSql = new ReusableSql(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserForRegistrationDto, UserComplete>();
        }));
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password == userForRegistration.PasswordConfirm)
        {
            string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if (existingUsers.Count() == 0)
            {
                UserForLoginDto userForSetPassword = new UserForLoginDto()
                {
                    Email = userForRegistration.Email,
                    Password = userForRegistration.Password
                };
                
                if (_authHelper.SetPassword(userForSetPassword))
                {
                    // string sqlAddUser = @"INSERT INTO TutorialAppSchema.Users([FirstName],[LastName],[Email],[Gender],[Active]) 
                    //     VALUES (" + "'" + userForRegistration.FirstName + "', '" + userForRegistration.LastName + "', '" + userForRegistration.Email + "', '" + userForRegistration.Gender + "', 1)";
                    // string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert @FirstName = '" + userForRegistration.FirstName + "', @LastName = '" + userForRegistration.LastName + "', @Email = '" + userForRegistration.Email + "', @Gender = '" + userForRegistration.Gender + "', @Active = 1" + ", @JobTitle = '" + userForRegistration.JobTitle + "', @Department = '" + userForRegistration.Department + "', @Salary = '" + userForRegistration.Salary + "'";
                    
                    UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                    userComplete.Active = true;

                    if (_reusableSql.UpsertUser(userComplete))
                    {
                        return Ok();
                    }

                    throw new Exception("Failed to add user");
                }

                throw new Exception("Failed to register user.");
            }

            throw new Exception("User with this email already exists!");
        }

        throw new Exception("Passwords do not match!");
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (_authHelper.SetPassword(userForSetPassword))
        {
            return Ok();
        }

        throw new Exception("Failed to reset password");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt =
            @"Exec TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

        DynamicParameters sqlParameters = new DynamicParameters();
        
        sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

        UserForLoginConfirmationDto userForLoginConfirmation =
            _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
            {
                return StatusCode(401,"Incorrect Password!");
            }
        }

        string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";
        
        int userId = _dapper.LoadDataSingle<int>(userIdSql);
        
        return Ok(new Dictionary<string, string>
        {
            {"token", _authHelper.CreateToken(userId)}
        });
    }

    
   [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        string userIdSql = @"SELECT UserId FROM TutorialAPpSchema.Users WHERE UserId = '" +
                           User.FindFirst("userId")?.Value + "'";
        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return _authHelper.CreateToken(userId);
    }
}