using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.ViewModels
{
    public class VerifyClaimViewModel
    {
        public int ClaimID { get; set; }

        [Required(ErrorMessage = "Please select an action")]
        public string Action { get; set; } // "Verify" or "Reject"

        [RequiredIf("Action", "Reject", ErrorMessage = "Rejection reason is required")]
        [StringLength(500)]
        public string RejectionReason { get; set; }
    }
}
