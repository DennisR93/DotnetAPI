namespace DotnetAPI.Interfaces;

public interface IUserRepository
{
    public bool SaveChanges();
    public void AddEntity<T>(T addEntity);
    public void RemoveEntity<T>(T deleteEntity);
}