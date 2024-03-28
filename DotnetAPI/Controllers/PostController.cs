using System.Data;
using Dapper;
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

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId = 0 , int userId = 0, string searchParam = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (postId != 0)
        {
            stringParameters += ", @PostId=@PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
        }

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }

        if (searchParam.ToLower() != "none")
        {
            stringParameters += ", @SearchValue=@SearchParameter";
            sqlParameters.Add("@SearchParameter", searchParam, DbType.String);
        }

        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }
        
        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }

    // [HttpGet("PostsSingle/{postId}")]
    // public Post GetSinglePost(int postId)
    // {
    //     string sql = @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE PostId =" + postId.ToString();
    //     return _dapper.LoadDataSingle<Post>(sql);
    // }
    
    // [HttpGet("PostsByUser/{userId}")]
    // public IEnumerable<Post> GetPostsByUser(int userId)
    // {
    //     string sql = @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId =" + userId.ToString();
    //     return _dapper.LoadData<Post>(sql);
    // }
    
    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParameter" ;
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }

    [HttpPut("UpsertPost")]
    public IActionResult UpsertPost(Post postToUpsert)
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Upsert @UserId = @UserIdParameter,
                      @PostTitle = @PostTitleParameter, @PostContent = @PostContentParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        
        sqlParameters.Add("UserIdParameter",  this.User.FindFirst("userId")?.Value, DbType.Int32);
        sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
        sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);
        
        if (postToUpsert.PostId > 0)
        {
            sql += ", @PostId = @PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
        }

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to create new post!");
    }
    
    // [HttpPut("Posts")]
    // public IActionResult EditPost(PostToEditDto postToEditDto)
    // {
    //     string sql =
    //         @"UPDATE TutorialAppSchema.Posts SET PostContent = '" + postToEditDto.PostContent + "', PostTitle='" +
    //         postToEditDto.PostTitle + "', PostUpdated = GETDATE() WHERE PostId = " + postToEditDto.PostId.ToString() + "AND UserId = " + this.User.FindFirst("userId")?.Value;
    //
    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }
    //
    //     throw new Exception("Failed to edit post!");
    // }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = "EXEC TutorialAppSchema.spPosts_Delete @PostId = @PostIdParameter, @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        
        sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
        
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to delete post!");
    }
    
    // [HttpGet("PostsBySearch/{searchParam}")]
    // public IEnumerable<Post> PostsBySearch(string searchParam)
    // {
    //     string sql =
    //         @"SELECT [PostId],[UserId],[PostTitle],[PostContent],[PostCreated],[PostUpdated] FROM TutorialAppSchema.Posts WHERE PostTitle LIKE '%" + searchParam + "%' OR PostContent LIKE '%" + searchParam + "%'";
    //     return _dapper.LoadData<Post>(sql);
    // }
}