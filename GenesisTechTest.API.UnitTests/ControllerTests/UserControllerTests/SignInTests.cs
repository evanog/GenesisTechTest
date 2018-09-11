using GenesisTechTest.API.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GenesisTechTest.API.UnitTests.ControllerTests.UserControllerTests
{
    [TestFixture]
    public class SignInTests : BaseUserControllerTests
    {
        [Test]
        public void Sign_In_Invalid_Email_Password_Response()
        {
            var request = new SignInRequest()
            {
                email = _invalidMockUser.Email,
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
                email = _validMockUser.Email,
                password = "password"
            };

            var response = _controller.SignIn(request);
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}