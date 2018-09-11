using System;
using System.Collections.Generic;
using AutoMapper;
using GenesisTechTest.API.Controllers;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GenesisTechTest.API.UnitTests.ControllerTests
{
    [TestFixture]
    public class SignInControllerTests
    {
        private readonly SignInController _controller;
        private readonly User mockuser;
        private readonly string invalidUserEmail;

        public SignInControllerTests()
        {
            invalidUserEmail = "invalidUserEmail@test.com";
            mockuser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "real@test.com"
            };

            var mockLogger = new Mock<ILogger<SignInController>>();
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(p => p.GetByEmailAndPasswordOrDefault(mockuser.Email, It.IsAny<string>())).Returns(mockuser);
            mockUserService.Setup(p => p.GetByEmailAndPasswordOrDefault(invalidUserEmail, It.IsAny<string>())).Throws(new InvalidEmailAndPasswordException());

            _controller = new SignInController(mockLogger.Object, mockMapper.Object, mockUserService.Object);
        }

        [Test]
        public void Sign_In_Invalid_Email_Password_Response()
        {
            var request = new SignInRequest()
            {
                email = invalidUserEmail,
                password = "password"
            };

            var response = _controller.SignIn(request);
            var result = response as ObjectResult;
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("Invalid user and / or password", ((ErrorResponse)result.Value).message);
        }

        [Test]
        public void Sign_In_Success()
        {
            var request = new SignInRequest()
            {
                email = mockuser.Email,
                password = "password"
            };

            var response = _controller.SignIn(request);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}