using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ContractMonthlyClaimSystem.Models
{
    public class Lecture
    {
        public Lecture() { }
        public int LecturerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string FullName => $"{FirstName} {LastName}";

        public DateTime DateJoined { get; set; }

        // Navigation property
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}





































