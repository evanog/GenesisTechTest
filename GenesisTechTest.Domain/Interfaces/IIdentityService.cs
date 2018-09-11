using System;

namespace GenesisTechTest.Domain.Interfaces
{
    public interface IIdentityService
    {
        string GetToken(string email, string password);
    }
}
