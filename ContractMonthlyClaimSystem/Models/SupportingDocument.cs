namespace ContractMonthlyClaimSystem.Models
{
    public class SupportingDocument
    {
        public int DocumentID { get; set; }
        public int ClaimID { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
}