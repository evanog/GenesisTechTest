using GenesisTechTest.Common.Models;

namespace GenesisTechTest.Domain.Interfaces
{
    public interface IValidationService
    {
        bool IsTokenMatching(User user, string token);
        bool IsWithinNumOfMinutes(User user, int minutes);
    }
}
