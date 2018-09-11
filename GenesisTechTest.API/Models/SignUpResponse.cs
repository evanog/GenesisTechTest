using System;

namespace GenesisTechTest.API.Models
{
    public class SignUpResponse
    {
        public Guid id { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime lastUpdatedOn { get; set; }
        public DateTime lastLoginOn { get; set; }
        public string token { get; set; }
    }
}