using DotnetAPI.Interfaces;

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