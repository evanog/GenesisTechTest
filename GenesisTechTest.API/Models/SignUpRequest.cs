using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenesisTechTest.API.Models
{
    public class SignUpRequest
    {
        [Required]
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required, MinLength(1)]
        public List<int> telephones { get; set; }
    }
}
