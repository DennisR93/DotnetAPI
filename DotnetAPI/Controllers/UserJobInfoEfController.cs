using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route(("[controller]"))]
public class UserJobInfoEfController : ControllerBase
{
    private DataContextEF _entityFramework;
    private IMapper _mapper;

    public UserJobInfoEfController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserJobInfoToAddDto, UserJobInfo>();
        }));
    }

    [HttpGet("GetUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetUsersJobInfo()
    {
        IEnumerable<UserJobInfo> usersJob = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
        return usersJob;
    }

    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault();
        if (userJobInfo != null)
        {
            return userJobInfo;
        }

        throw new Exception("Failed to Get User Job Info");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userDb = _entityFramework.UserJobInfo.Where(u => u.UserId == userJobInfo.UserId).FirstOrDefault();
        if (userDb != null)
        {
            userDb.JobTitle = userJobInfo.JobTitle;
            userDb.Department = userJobInfo.Department;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }

        throw new Exception("Failed to Edit User Job Info");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfoToAddDto userJobInfoToAdd)
    {
        UserJobInfo userDb = _mapper.Map<UserJobInfo>(userJobInfoToAdd);

        userDb.JobTitle = userJobInfoToAdd.JobTitle;
        userDb.Department = userJobInfoToAdd.Department;

        _entityFramework.Add(userDb);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Job Info");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? userDb = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault<UserJobInfo>();
        if (userDb != null)
        {
            _entityFramework.UserJobInfo.Remove(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User Job Info");
        }

        throw new Exception("Failed to Get User Job Info");
    }
}