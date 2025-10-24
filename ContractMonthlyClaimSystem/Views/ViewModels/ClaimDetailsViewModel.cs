namespace ContractMonthlyClaimSystem.ViewModels
{
    public class ClaimDetailsViewModel
    {
        public int ClaimID { get; set; }
        public string LecturerName { get; set; }
        public string LecturerEmail { get; set; }
        public string Department { get; set; }
        public string Month { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public List<SupportingDocument> Documents { get; set; }
    }
}
