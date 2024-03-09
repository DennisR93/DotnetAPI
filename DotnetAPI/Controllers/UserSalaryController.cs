using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserSalaryController : ControllerBase
{
    private DataContextDapper _dapper;
    public UserSalaryController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
    
    [HttpGet("GetUserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(@"SELECT [UserId],[UserSalary] FROM TutorialAppSchema.UserSalary WHERE UserId = " + userId.ToString());
        
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"UPDATE TutorialAppSchema.UserSalary SET [Salary] =" + userSalary.Salary + " WHERE UserId = " + userSalary.UserId;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Update User Salary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalaryToAdd)
    {

        string sql = @"INSERT INTO TutorialAppSchema.UserSalary([UserId],[Salary]) VALUES (" + userSalaryToAdd.UserId + ", " + userSalaryToAdd.Salary + ")";
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.UserSalary WHERE UserId =" + userId.ToString();

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User Salary");
    }
    
}