using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route(("[controller]"))]
public class UserSalaryEfController : ControllerBase
{
    private DataContextEF _entityFramework;
    private IMapper _mapper;

    public UserSalaryEfController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserSalaryToAddDto, UserSalary>();
        }));
    }

    [HttpGet("GetUsersSalary")]
    public IEnumerable<UserSalary> GetUsersSalary()
    {
        IEnumerable<UserSalary> userSalary = _entityFramework.UserSalary.ToList<UserSalary>();
        return userSalary;
    }

    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault();
        if (userSalary != null)
        {
            return userSalary;
        }

        throw new Exception("Failed to Get User Salary");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userDb = _entityFramework.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault();
        if (userDb != null)
        {
            userDb.Salary = userSalary.Salary;
            userDb.AvgSalary = userSalary.AvgSalary;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }

        throw new Exception("Failed to Edit User Salary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalaryToAddDto userSalaryToAdd)
    {
        UserSalary userDb = _mapper.Map<UserSalary>(userSalaryToAdd);

        userDb.Salary = userSalaryToAdd.Salary;
        userDb.AvgSalary = userSalaryToAdd.AvgSalary;

        _entityFramework.Add(userDb);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpDelete("DeleteUserJSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userDb = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();
        if (userDb != null)
        {
            _entityFramework.UserSalary.Remove(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User Job Info");
        }

        throw new Exception("Failed to Get User Job Info");
    }
}