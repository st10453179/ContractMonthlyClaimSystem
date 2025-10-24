namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public int LecturerID { get; set; }
        public string Month { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount => HoursWorked * HourlyRate;
        public int StatusID { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? VerifiedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public string RejectionReason { get; set; }


        // Navigation properties
        public Lecture Lecturer { get; set; }
        public ClaimStatus ClaimStatus { get; set; }
        public ICollection<SupportingDocument> SupportingDocuments { get; set; }
    }
}

