using GenesisTechTest.Common.Models;

namespace GenesisTechTest.Domain.Interfaces
{
    public interface IValidatorService
    {bool IsWithinNumOfMinutes(User user, int minutes);
    }
}
