using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

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
}