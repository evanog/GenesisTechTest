using System;
using GenesisTechTest.Common.Models;

namespace GenesisTechTest.DataAccess.Interfaces
{
    public interface IStorageRepository
    {
        User GetByUserIdOrDefault(Guid userid);
        User GetByEmailOrDefault(string emaild);
        void Create(User user);
        void UpdateLastLogin(Guid id, DateTime lastLogin, string token);
        bool IsEmailAlreadyExists(string email);
    }
}
