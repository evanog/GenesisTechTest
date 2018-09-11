using System;
using AutoMapper;
using GenesisTechTest.API.Controllers;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Models;
using GenesisTechTest.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace GenesisTechTest.API.UnitTests.ControllerTests
{
    [TestFixture]
    public class SearchControllerTests
    {
        private readonly SearchController _controller;
        private readonly User mockuser;
        private readonly User invalidMockUser;
        private readonly Guid nullMockUserGuid;

        public SearchControllerTests()
        {
            nullMockUserGuid = Guid.NewGuid();
            mockuser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "real@test.com"
            };
            invalidMockUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "invalid@test.com"
            };
            
            var mockLogger = new Mock<ILogger<SearchController>>();
            var mockMapper = new Mock<IMapper>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(p => p.GetByUserIdOrDefault(mockuser.Id)).Returns(mockuser);
            mockUserService.Setup(p => p.GetByUserIdOrDefault(nullMockUserGuid)).Returns((User)null);
            mockUserService.Setup(p => p.GetByUserIdOrDefault(invalidMockUser.Id)).Returns(invalidMockUser);
            var mockValidatorService = new Mock<IValidatorService>();
            mockValidatorService.Setup(p => p.IsWithinNumOfMinutes(invalidMockUser, It.IsAny<int>())).Returns(false);
            mockValidatorService.Setup(p => p.IsWithinNumOfMinutes(mockuser, It.IsAny<int>())).Returns(true);

            var config = Options.Create(new AppConfig());

            _controller = new SearchController(mockLogger.Object, mockMapper.Object, mockUserService.Object, mockValidatorService.Object, config);
        }

        [Test]
        public void Search_For_User_With_Invalid_Guid_Response()
        {
           
            var response = _controller.Get(nullMockUserGuid);
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void Search_For_User_With_Invalid_Session()
        {
            var response = _controller.Get(invalidMockUser.Id);
            var result = response as ObjectResult;
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("Invalid Session", ((ErrorResponse)result.Value).message);
        }
        
        [Test]
        public void Get_Success()
        {
            var response = _controller.Get(mockuser.Id);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}