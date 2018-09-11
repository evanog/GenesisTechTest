using System;
using AutoMapper;
using GenesisTechTest.API.Controllers;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GenesisTechTest.API.UnitTests.ControllerTests.UserControllerTests
{
    public class BaseUserControllerTests
    {
        public readonly User _validMockUser;
        public readonly User _invalidMockUser;
        public readonly User _invalidSessionMockUser;
        public readonly User _nullMockUser;
        public readonly UserController _controller;
        public readonly Mock<IUserService> _mockUserService;

        public BaseUserControllerTests()
        {
            _validMockUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "real@test.com",
                Token = "USERTOKEN"
            };
            _invalidMockUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "invalid@test.com",
            };
            _invalidSessionMockUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "invalid@test.com",
                Token = "INVALIDSESESSION"
            };
            _nullMockUser = new User()
            {
                Id = Guid.NewGuid()
            };

            var mockLogger = new Mock<ILogger<UserController>>();
            var mockMapper = new Mock<IMapper>();

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(p => p.GetByUserIdOrDefault(_validMockUser.Id)).Returns(_validMockUser);
            _mockUserService.Setup(p => p.GetByUserIdOrDefault(_invalidMockUser.Id)).Returns(_invalidMockUser);
            _mockUserService.Setup(p => p.GetByUserIdOrDefault(_invalidSessionMockUser.Id)).Returns(_invalidSessionMockUser);
            _mockUserService.Setup(p => p.GetByUserIdOrDefault(_nullMockUser.Id)).Returns((User)null);
            _mockUserService.Setup(p => p.SignIn(_validMockUser.Email, It.IsAny<string>())).Returns(_validMockUser);
            _mockUserService.Setup(p => p.SignIn(_invalidMockUser.Email, It.IsAny<string>())).Throws(new InvalidEmailAndPasswordException());

            var mockValidatorService = new Mock<IValidationService>();
            mockValidatorService.Setup(p => p.IsWithinNumOfMinutes(_invalidMockUser, It.IsAny<int>())).Returns(false);
            mockValidatorService.Setup(p => p.IsWithinNumOfMinutes(_validMockUser, It.IsAny<int>())).Returns(true);
            mockValidatorService.Setup(p => p.IsTokenMatching(It.IsAny<User>(), "USERTOKEN")).Returns(true);
            mockValidatorService.Setup(p => p.IsTokenMatching(_invalidMockUser, "USERTOKEN")).Returns(false);
            mockValidatorService.Setup(p => p.IsTokenMatching(_invalidSessionMockUser, "INVALIDSESESSION")).Returns(true);

            var config = Options.Create(new AppConfig());

            _controller = new UserController(mockLogger.Object, mockMapper.Object, _mockUserService.Object, mockValidatorService.Object, config);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "USERTOKEN";
        }
    }
}
