using System;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Services;
using NUnit.Framework;

namespace GenesisTechTest.Domain.UnitTests.ServicesTests
{
    [TestFixture]
    public class ValidatorServiceTests
    {
        private readonly ValidationService _service;

        public ValidatorServiceTests()
        {
            _service = new ValidationService();
        }

        [Test]
        public void IsWithinNumOfMinutes_Fails_When_Greater_30_Mins()
        {
            var user = new User()
            {
                LastLoginOn = DateTime.UtcNow.AddMinutes(-40)
            };

            var isValid = _service.IsWithinNumOfMinutes(user, 30);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsWithinNumOfMinutes_Fails_When_Less_30_Mins()
        {
            var user = new User()
            {
                LastLoginOn = DateTime.UtcNow.AddMinutes(-10)
            };

            var isValid = _service.IsWithinNumOfMinutes(user, 30);

            Assert.IsTrue(isValid);
        }
    }
}