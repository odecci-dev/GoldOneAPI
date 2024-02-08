using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class UserVM
    {
        public int Id { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Mname { get; set; }
        public string? Suffix { get; set; }
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Cno { get; set; }
        public string? Address { get; set; }
        public string? UserId { get; set; }
        public string? UserTypeId { get; set; }
        public string? UserType { get; set; }
        public string? StatusName { get; set; }
        public string? DateCreated { get; set; }
        public string? DateUpdated { get; set; }
        public string? ProfilePath { get; set; }
        public string? FOID { get; set; }
        public string? FieldOfficer { get; set; }

    }
}

