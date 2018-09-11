using System;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.DataAccess.Interfaces;
using GenesisTechTest.Domain.Interfaces;
using GenesisTechTest.Domain.Services;
using Moq;
using NUnit.Framework;

namespace GenesisTechTest.Domain.UnitTests.ServicesTests
{

    [TestFixture]
    public class UserServiceTests
    {
        private readonly UserService _service;
        private readonly string invalidEmail;
        private readonly DateTime lastLogin;
        private readonly User validUser;
        private readonly Guid invalidUser;

        public UserServiceTests()
        {
            invalidUser = Guid.NewGuid();
            invalidEmail = "invalid@test.com";
            lastLogin = DateTime.UtcNow.AddYears(-1);

            validUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "valid@test.com",
                LastLoginOn = lastLogin
            };

            var mockStorage = new Mock<IStorageRepository>();
            mockStorage.Setup(p => p.GetByUserIdOrDefault(validUser.Id)).Returns(validUser);
            mockStorage.Setup(p => p.GetByUserIdOrDefault(invalidUser)).Returns((User)null);
            mockStorage.Setup(p => p.GetByEmailOrDefault(validUser.Email)).Returns(validUser);
            mockStorage.Setup(p => p.GetByEmailOrDefault(invalidEmail)).Returns((User)null);
            mockStorage.Setup(p => p.IsEmailAlreadyExists(validUser.Email)).Returns(true);
            mockStorage.Setup(p => p.IsEmailAlreadyExists(invalidEmail)).Returns(false);

            var mockIdenityService = new Mock<IIdentityService>();
            mockIdenityService.Setup(p => p.GetToken(It.IsAny<string>(), It.IsAny<string>())).Returns("token");

            var mockPasswordHashService = new Mock<IPasswordHashService>();
            mockPasswordHashService.Setup(p => p.Verify("valid", It.IsAny<string>())).Returns(true);
            mockPasswordHashService.Setup(p => p.Verify("invalid", It.IsAny<string>())).Returns(false);

            _service = new UserService(mockStorage.Object, mockIdenityService.Object, mockPasswordHashService.Object);
        }

        [Test]
        public void Can_Get_User_By_ID()
        {
            var result = _service.GetByUserIdOrDefault(validUser.Id);
            Assert.NotNull(result);
        }

        [Test]
        public void Can_Get_User_By_ID_Not_Found()
        {
            var result = _service.GetByUserIdOrDefault(invalidUser);
            Assert.IsNull(result);
        }

        [Test]
        public void Can_Get_User_Credentials_Invalid_EMail_Exception()
        {
            Assert.Throws<InvalidEmailAndPasswordException>(() => _service.SignIn(invalidEmail, "password"));
        }

        [Test]
        public void Can_Get_User_Credentials_Invalid_Password_Exception()
        {
            Assert.Throws<InvalidEmailAndPasswordException>(() => _service.SignIn(validUser.Email, "invalid"));
        }

        [Test]
        public void Can_Get_User_Credentials_Success()
        {
            var result = _service.SignIn(validUser.Email, "valid");
            Assert.IsTrue(result.LastLoginOn > lastLogin);
            Assert.IsNotNull(result.Token);
        }

        [Test]
        public void Can_Create_User_Email_Already_Exists()
        {
            Assert.Throws<EmailAlreadyExistsException>(() => _service.CreateUser(validUser));
        }

        [Test]
        public void Can_Create_User_Success()
        {
            var newUser = new User()
            {
                Password = "testpassword"
            };

            var result = _service.CreateUser(newUser);
            Assert.IsNotNull(result.Id);
            Assert.IsNotNull(result.CreatedOn);
            Assert.IsNotNull(result.LastUpdatedOn);
            Assert.IsNotNull(result.LastLoginOn);
            Assert.IsNotNull(result.Token);
        }

    }
}
