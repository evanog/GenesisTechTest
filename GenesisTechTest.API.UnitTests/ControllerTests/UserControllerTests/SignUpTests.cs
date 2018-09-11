using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Exceptions;
using GenesisTechTest.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace GenesisTechTest.API.UnitTests.ControllerTests.UserControllerTests
{
    [TestFixture]
    public class SignUpTests : BaseUserControllerTests
    {
        [Test]
        public void Sign_In_Email_Already_Exists_Response()
        {
            _mockUserService.Setup(p => p.CreateUser(It.IsAny<User>())).Throws(new EmailAlreadyExistsException());

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
            _mockUserService.Setup(p => p.CreateUser(It.IsAny<User>())).Returns(_validMockUser);

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