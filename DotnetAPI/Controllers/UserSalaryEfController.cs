using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Interfaces;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route(("[controller]"))]
public class UserSalaryEfController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private DataContextEF _entityFramework;
    private IMapper _mapper;

    public UserSalaryEfController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserSalary, UserSalary>();
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
        UserSalary? userSalaryToUpdate = _entityFramework.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault();
        if (userSalaryToUpdate != null)
        {
            _mapper.Map(userSalary, userSalaryToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Updating UserSalary failed to save");
        }

        throw new Exception("Failed to Find UserSalary to Update");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalaryToAdd)
    {
        // UserSalary userDb = _mapper.Map<UserSalary>(userSalaryToAdd);
        //
        // userDb.Salary = userSalaryToAdd.Salary;
        // userDb.AvgSalary = userSalaryToAdd.AvgSalary;

        _userRepository.AddEntity<UserSalary>(userSalaryToAdd);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpDelete("DeleteUserJSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userToDelete = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();
        if (userToDelete != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete UserSalary Info");
        }

        throw new Exception("Failed to Get UserSalary to Delete");
    }
}