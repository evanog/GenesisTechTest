using GenesisTechTest.API.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GenesisTechTest.API.UnitTests.ControllerTests.UserControllerTests
{
    [TestFixture]
    public class SearchForUserTests : BaseUserControllerTests
    {
        [Test]
        public void Search_For_User_With_Invalid_Guid_Response()
        {
            var response = _controller.Get("fakeguid");
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void Search_For_User_With_Guid_Not_Existing_Response()
        {
            var response = _controller.Get(_nullMockUser.Id.ToString());
            var result = response as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void Search_For_User_With_Not_Matching_Token()
        {
            var response = _controller.Get(_invalidMockUser.Id.ToString());
            var result = response as ObjectResult;
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("Unauthorized", ((ErrorResponse)result.Value).message);
        }

        [Test]
        public void Search_For_User_With_Invalid_Session()
        {
            var response = _controller.Get(_invalidSessionMockUser.Id.ToString());
            var result = response as ObjectResult;
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("Invalid Session", ((ErrorResponse)result.Value).message);
        }

        [Test]
        public void Get_Success()
        {
            var response = _controller.Get(_validMockUser.Id.ToString());
            var result = response as OkObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}