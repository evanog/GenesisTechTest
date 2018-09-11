using System;
using GenesisTechTest.Common.Models;

namespace GenesisTechTest.DataAccess.Interfaces
{
    public interface IStorageRepository
    {
        User GetByUserIdOrDefault(Guid userid);
        User GetByEmailOrDefault(string emaild);
        void Create(User user);
        bool SetLastLogin(Guid id, DateTime lastLogin);
        bool IsEmailAlreadyExists(string email);
    }
}
