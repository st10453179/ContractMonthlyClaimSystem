using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } // Generated, Paid

        // Navigation property
        public virtual ICollection<Claim> Claims { get; set; }
    }
}

