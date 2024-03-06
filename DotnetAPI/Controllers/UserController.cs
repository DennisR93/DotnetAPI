using DotnetAPI.Data;
using DotnetAPI.DTOs;
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
    
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"SELECT [UserId],[FirstName],[LastName],[Email],[Gender],[Active] FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"UPDATE TutorialAppSchema.Users SET [FirstName] = '" + user.FirstName + "', [LastName] = '" + user.LastName + "', [Email] = '" + user.Email + "', [Gender] = '" + user.Gender + "', [Active] = '" + user.Active + "' WHERE UserId = " + user.UserId;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto userToAdd)
    {

        string sql = @"INSERT INTO TutorialAppSchema.Users(
[FirstName],
[LastName],
[Email],
[Gender],
[Active]) VALUES (" + "'" + userToAdd.FirstName + "', '" + userToAdd.LastName + "', '" + userToAdd.Email + "', '" + userToAdd.Gender + "', '" + userToAdd.Active + "')";
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Users WHERE UserId =" + userId.ToString();

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }
    
}