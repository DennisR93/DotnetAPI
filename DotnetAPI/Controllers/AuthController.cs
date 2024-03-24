using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Interfaces;
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
    
    public AuthController(IConfiguration config, AuthHelper authHelper)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = authHelper;
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
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                string sqlAddAuth = "EXEC TutorialAppSchema.spRegistration_Upsert @Email = @EmailParam, @PasswordHash = @PasswordHashParam, @PasswordSalt = @PasswordSaltParam";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                
                SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
                emailParameter.Value = userForRegistration.Email;
                sqlParameters.Add(emailParameter);
                
                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;
                sqlParameters.Add(passwordHashParameter);

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;
                sqlParameters.Add(passwordSaltParameter);

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert @FirstName = '" + userForRegistration.FirstName + "', @LastName = '" + userForRegistration.LastName + "', @Email = '" + userForRegistration.Email + "', @Gender = '" + userForRegistration.Gender + "', @Active = 1" + ", @JobTitle = '" + userForRegistration.JobTitle + "', @Department = '" + userForRegistration.Department + "', @Salary = '" + userForRegistration.Salary + "'";
                    
                    // string sqlAddUser = @"INSERT INTO TutorialAppSchema.Users([FirstName],[LastName],[Email],[Gender],[Active]) 
                    //     VALUES (" + "'" + userForRegistration.FirstName + "', '" + userForRegistration.LastName + "', '" + userForRegistration.Email + "', '" + userForRegistration.Gender + "', 1)";

                    if (_dapper.ExecuteSql(sqlAddUser))
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

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt =
            @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation =
            _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

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