using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT [UserId],[FirstName],[LastName],[Email],[Gender],[Active] FROM TutorialAppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;

    }
    
    // [HttpGet("GetSingleUser/{userId}")]
    // public User GetSingleUser(int userId)
    // {
    //     
    // }
}