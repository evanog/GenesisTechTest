using System;
using GenesisTechTest.Common.Models;

namespace GenesisTechTest.Domain.Interfaces
{
    public interface IUserService
    {
        User GetByUserIdOrDefault(Guid userId);
        User GetByEmailAndPasswordOrDefault(string email, string password);
        User CreateUser(User user);
    }
}
