using System.ComponentModel.DataAnnotations;

namespace GenesisTechTest.API.Models
{
    public class SignInRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}