using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Interfaces;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route(("[controller]"))]
public class UserJobInfoEfController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private IMapper _mapper;

    public UserJobInfoEfController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));
    }

    // [HttpGet("GetUsersJobInfo")]
    // public IEnumerable<UserJobInfo> GetUsersJobInfo()
    // {
    //     IEnumerable<UserJobInfo> usersJob = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
    //     return usersJob;
    // }

    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userJobInfo.UserId);
        if (userToUpdate != null)
        {
            _mapper.Map(userToUpdate, userJobInfo);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        }

        throw new Exception("Failed to Edit User Job Info");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfoToAdd)
    {
        _userRepository.AddEntity<UserJobInfo>(userJobInfoToAdd);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Job Info");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? userDb = _userRepository.GetSingleUserJobInfo(userId);
        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User Job Info");
        }
        throw new Exception("Failed to Get User Job Info");
    }
}