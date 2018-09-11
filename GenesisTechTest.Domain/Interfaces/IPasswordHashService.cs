namespace GenesisTechTest.Domain.Interfaces
{
    public interface IPasswordHashService
    {
        string GetHashedPassword(string password);
        bool Verify(string password, string hashValue);
    }
}
