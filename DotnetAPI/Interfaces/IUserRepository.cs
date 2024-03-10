using DotnetAPI.Models;

namespace DotnetAPI.Interfaces;

public interface IUserRepository
{
    public bool SaveChanges();
    public void AddEntity<T>(T addEntity);
    public void RemoveEntity<T>(T deleteEntity);

    public IEnumerable<User> GetUsers();
    public User GetSingleUser(int userId);

    public UserSalary GetSingleUserSalary(int userId);
    public UserJobInfo GetSingleUserJobInfo(int userId);
}