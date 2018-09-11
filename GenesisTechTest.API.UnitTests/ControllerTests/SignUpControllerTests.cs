using System;
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
    public class SignUpControllerTests
    {
        
        private readonly SignUpController _controller;
        private readonly Mock<IUserService> mockUserService;
        private readonly User mockuser;

        public SignUpControllerTests()
        {
            mockuser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "real@test.com"
            };

            var mockLogger = new Mock<ILogger<SignUpController>>();
            var mockMapper = new Mock<IMapper>();
            mockUserService = new Mock<IUserService>();
            

            _controller = new SignUpController(mockLogger.Object, mockMapper.Object, mockUserService.Object);
        }

        [Test]
        public void Sign_In_Email_Already_Exists_Response()
        {
            mockUserService.Setup(p => p.CreateUser(It.IsAny<User>())).Throws(new EmailAlreadyExistsException());

            var request = new SignUpRequest()
            {
                email = "email",
                password = "password"
            };

            var response = _controller.SignUp(request);
            var result = response as ConflictObjectResult;
            Assert.AreEqual(409, result.StatusCode);
            Assert.AreEqual("E-mail already exists", ((ErrorResponse)result.Value).message);
        }

        [Test]
        public void Sign_In_Success()
        {
            mockUserService.Setup(p => p.CreateUser(It.IsAny<User>())).Returns(mockuser);

            var request = new SignUpRequest()
            {
                email = "email",
                password = "password"
            };

            var response = _controller.SignUp(request);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}