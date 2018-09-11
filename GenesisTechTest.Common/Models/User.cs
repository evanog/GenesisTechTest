using System;
using System.Collections.Generic;

namespace GenesisTechTest.Common.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public DateTime LastLoginOn { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<int> Telephones { get; set; }
        public string Token { get; set; }
    }
}
