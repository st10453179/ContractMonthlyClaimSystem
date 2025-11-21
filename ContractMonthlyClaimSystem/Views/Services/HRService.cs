using System.Data.Entity;
using System.Text;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;

namespace ContractClaimSystem.Services
{
    public interface IHRService
    {
        Task<Invoice> GenerateInvoice(List<int> claimIds);
        Task<List<Claim>> GetApprovedClaimsForMonth(int year, int month);
        Task<byte[]> GeneratePaymentReport(int year, int month);
    }

    public class HRService : IHRService
    {
        private readonly ApplicationDbContext _context;

        public HRService(ApplicationDbContext context)
        {
            _context = context;
        }

        // AUTOMATION 4: Auto-generate invoices
        public async Task<Invoice> GenerateInvoice(List<int> claimIds)
        {
            var claims = await _context.Claims
                .Where(c => claimIds.Contains(c.ClaimId) && c.Status == "Approved")
                .Include(c => c.Lecture)
                .ToListAsync();

            if (!claims.Any())
            {
                throw new Exception("No approved claims found");
            }

            var invoice = new Invoice
            {
                InvoiceDate = DateTime.Now,
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                TotalAmount = claims.Sum(c => c.TotalPayment),
                Status = "Generated",
                Claims = claims
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        // Get approved claims for a specific month
        public async Task<List<Claim>> GetApprovedClaimsForMonth(int year, int month)
        {
            return await _context.Claims
                .Include(c => c.Lecture)
                .Where(c => c.Status == "Approved" &&
                            c.ApprovalDate.HasValue &&
                            c.ApprovalDate.Value.Year == year &&
                            c.ApprovalDate.Value.Month == month)
                .ToListAsync();
        }

        // AUTOMATION 5: Generate payment report
        public async Task<byte[]> GeneratePaymentReport(int year, int month)
        {
            var claims = await GetApprovedClaimsForMonth(year, month);

            // Here you would use a reporting library like Crystal Reports or SSRS
            // For now, we'll create a simple CSV format
            var csv = new StringBuilder();
            csv.AppendLine("Lecturer Name,Email,Hours Worked,Hourly Rate,Total Payment,Approval Date");

            foreach (var claim in claims)
            {
                csv.AppendLine($"{claim.Lecture.FirstName} {claim.Lecture.LastName}," +
                              $"{claim.Lecture.Email}," +
                              $"{claim.HoursWorked}," +
                              $"{claim.HourlyRate}," +
                              $"{claim.TotalPayment}," +
                              $"{claim.ApprovalDate:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}