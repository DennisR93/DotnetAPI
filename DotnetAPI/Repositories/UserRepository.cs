using DotnetAPI.Interfaces;
using DotnetAPI.Models;

namespace DotnetAPI.Data;

public class UserRepository : IUserRepository
{
    private DataContextEF _entityFramework;

    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        if (entityToAdd != null)
        {
            _entityFramework.Add(entityToAdd);
        }
    }

    public void RemoveEntity<T>(T entityToRemove)
    {
        if (entityToRemove != null)
        {
            _entityFramework.Remove(entityToRemove);
        }
    }
    
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;

    }
    
    public User GetSingleUser(int userId)
    {
        User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
        if (user != null)
        {
            return user;
        }

        throw new Exception("Failed to Get User");
    }
    
    //UserSalary
    
    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault();
        if (userSalary != null)
        {
            return userSalary;
        }

        throw new Exception("Failed to Get User Salary");
    }
    
    //UserJobInfo
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault();
        if (userJobInfo != null)
        {
            return userJobInfo;
        }

        throw new Exception("Failed to Get User Job Info");
    }

    //If we would like to use Boolean and didn't have the save changes.
    // public bool AddEntity<T>(T entityToAdd)
    // {
    //     if (entityToAdd != null)
    //     {
    //         _entityFramework.Add(entityToAdd);
    //         return true;
    //     }
    //
    //     return false;
    // }
}