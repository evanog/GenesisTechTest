using System;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;

namespace GenesisTechTest.Domain.Services
{
    public class ValidatorService : IValidatorService
    {
        public bool IsWithinNumOfMinutes(User user, int minutes)
        {
            var compare = DateTime.UtcNow.AddMinutes(minutes * -1);

            var span = DateTime.UtcNow.Subtract(user.LastLoginOn);

            return span.Minutes < 30;
        }
    }
}

