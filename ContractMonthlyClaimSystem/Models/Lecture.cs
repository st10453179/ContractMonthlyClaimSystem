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

        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
//