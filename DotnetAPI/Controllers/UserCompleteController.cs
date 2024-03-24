using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
    
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }

        if (isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }

        sql += parameters.Substring(1);
        
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;

    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        // TutorialAppSchema.spUser_Upsert
        // @FirstName NVARCHAR(50),
        // @LastName NVARCHAR(50),
        // @Email NVARCHAR(50),
        // @Gender NVARCHAR(50),
        // @JobTitle NVARCHAR(50),
        // @Department NVARCHAR(50),
        // @Salary DECIMAL(18, 4),
        // @Active BIT = 1,
        // @UserId INT = NULL
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert @FirstName = '" + user.FirstName + "', @LastName = '" + user.LastName + "', @Email = '" + user.Email + "', @Gender = '" + user.Gender + "', @Active = '" + user.Active + "', @JobTitle = '" + user.JobTitle + "', @Department = '" + user.Department + "', @Salary = '" + user.Salary + "', @UserId = " + user.UserId;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Update User");
    }
    

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.spUser_Delete @UserId =" + userId.ToString();

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }
    
}