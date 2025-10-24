using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Views.ViewModels
{
    public class SubmitClaimViewModel
    {
        [Required(ErrorMessage = "Month is required")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 744, ErrorMessage = "Hours must be between 1 and 744")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0.01, 10000, ErrorMessage = "Hourly rate must be greater than 0")]
        public double HourlyRate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string AdditionalNotes { get; set; }

        [Required(ErrorMessage = "Please upload at least one supporting document")]
        public IFormFile SupportingDocument { get; set; }

        public List<IFormFile> AdditionalDocuments { get; set; }
    }
}