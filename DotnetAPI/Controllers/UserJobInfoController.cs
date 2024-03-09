using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase
{
    private DataContextDapper _dapper;
    public UserJobInfoController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
    
    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetSingleUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(@"SELECT [UserId],[JobTitle],[Department] FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userId.ToString());
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo user)
    {
        string sql = @"UPDATE TutorialAppSchema.UserJobInfo SET [Department] = '" + user.Department + "', [JobTitle] = '" + user.JobTitle + "' WHERE UserId = " + user.UserId;
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userToAdd)
    {

        string sql = @"INSERT INTO TutorialAppSchema.UserJobInfo([UserId],[Department],[JobTitle]) VALUES (" + userToAdd.UserId + ", '" + userToAdd.Department + "', '" + userToAdd.JobTitle + "')";
        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId =" + userId.ToString();

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User Job Info");
    }
    
}