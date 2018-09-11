using System;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;

namespace GenesisTechTest.Domain.Services
{
    public class ValidationService : IValidationService
    {
        public bool IsTokenMatching(User user, string token)
        {
            return token.Contains(user.Token);
        }

        public bool IsWithinNumOfMinutes(User user, int minutes)
        {
            var span = DateTime.UtcNow.Subtract(user.LastLoginOn);
            return span.Minutes < minutes;
        }
    }
}

