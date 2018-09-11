using GenesisTechTest.Domain.Services;
using NUnit.Framework;

namespace GenesisTechTest.Domain.UnitTests.ServicesTests
{
    [TestFixture]
    public class PasswordHashServiceTests
    {
        private readonly PasswordHashService _service;

        public PasswordHashServiceTests()
        {
            _service = new PasswordHashService();
        }

        [Test]
        public void Password_Can_Be_Hashed_And_Verified()
        {
            var correctPassword = "password1234";
            var incorrectPassword = "asdfasdf";

            var hashedPassword = _service.GetHashedPassword(correctPassword);

            Assert.IsTrue(_service.Verify(correctPassword, hashedPassword));
            Assert.IsFalse(_service.Verify(correctPassword, incorrectPassword));
        }
    }
}
