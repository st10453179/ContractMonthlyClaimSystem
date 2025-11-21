using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required]
        public int LectureId { get; set; }

        [Required]
        [Range(1, 744, ErrorMessage = "Hours must be between 1 and 744 (max hours in a month)")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(100, 5000, ErrorMessage = "Hourly rate must be between R100 and R5000")]
        public decimal HourlyRate { get; set; }

        [Required]
        public decimal TotalPayment { get; set; }

        public DateTime SubmissionDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Pending, Approved, Rejected

        public string Notes { get; set; }

        public string RejectionReason { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string ApprovedBy { get; set; }

        // Navigation property
        public virtual Lecture Lecture { get; set; }

        // Supporting documents (optional)
        public string DocumentPath { get; set; }
        public int LecturerId { get; internal set; }
    }

}