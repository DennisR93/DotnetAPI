using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string sql =
            @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts";
        return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("PostsSingle/{postId}")]
    public Post GetSinglePost(int postId)
    {
        string sql = @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE PostId =" + postId.ToString();
        return _dapper.LoadDataSingle<Post>(sql);
    }
    
    [HttpGet("PostsByUser/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        string sql = @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId =" + userId.ToString();
        return _dapper.LoadData<Post>(sql);
    }
    
    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId =" + this.User.FindFirst("userId")?.Value;
        return _dapper.LoadData<Post>(sql);
    }

    [HttpPost("Posts")]
    public IActionResult AddPost(PostToAddDto postToAddDto)
    {
        string sql =
            @"INSERT INTO TutorialAppSchema.Posts([UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated]) VALUES (" +
            this.User.FindFirst("userId")?.Value + ",'" + postToAddDto.PostTitle + "','" + postToAddDto.PostContent +
            "', GETDATE(), GETDATE() )";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to create new post!");
    }
    
    [HttpPut("Posts")]
    public IActionResult EditPost(PostToEditDto postToEditDto)
    {
        string sql =
            @"UPDATE TutorialAppSchema.Posts SET PostContent = '" + postToEditDto.PostContent + "', PostTitle='" +
            postToEditDto.PostTitle + "', PostUpdated = GETDATE() WHERE PostId = " + postToEditDto.PostId.ToString() + "AND UserId = " + this.User.FindFirst("userId")?.Value;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to edit post!");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = "DELETE FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString() + "AND UserId =" + this.User.FindFirst("userId")?.Value;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to delete post!");
    }
}