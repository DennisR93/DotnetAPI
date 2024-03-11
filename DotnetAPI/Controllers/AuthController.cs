using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers;

public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _configuration;
    
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _configuration = config;
    }

    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password == userForRegistration.PasswordConfirm)
        {
            string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistration.Email + "'";
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if (existingUsers.Count() == 0)
            {
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                string sqlAddAuth =
                    @"INSERT INTO TutorialAppSchema.Auth ([Email], [PasswordHash], [PasswordSalt]) VALUES ('" +
                    userForRegistration.Email + "', @PasswordHash, @PasswordSalt)";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;
                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    return Ok();
                }

                throw new Exception("Failed to register user.");
            }

            throw new Exception("User with this email already exists!");
        }

        throw new Exception("Passwords do not match!");
    }

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt =
            @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = ''" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation =
            _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
            {
                return StatusCode(401,"Incorrect Password!");
            }
        }
        
        return Ok();
    }

    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );
    }
}