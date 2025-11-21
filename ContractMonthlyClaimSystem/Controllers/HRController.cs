using System.Data.Entity;
using ContractClaimSystem.Services;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractClaimSystem.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly IHRService _hrService;
        private readonly ApplicationDbContext _context;

        public HRController(IHRService hrService, ApplicationDbContext context)
        {
            _hrService = hrService;
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var approvedClaims = await _hrService.GetApprovedClaimsForMonth(currentYear, currentMonth);

            ViewBag.TotalClaims = approvedClaims.Count;
            ViewBag.TotalAmount = approvedClaims.Sum(c => c.TotalPayment);

            return View(approvedClaims);
        }

        // Generate invoice
        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(List<int> claimIds)
        {
            try
            {
                var invoice = await _hrService.GenerateInvoice(claimIds);
                TempData["Success"] = $"Invoice {invoice.InvoiceNumber} generated successfully!";

                return RedirectToAction("ViewInvoice", new { id = invoice.InvoiceId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        // Download payment report
        public async Task<IActionResult> DownloadReport(int year, int month)
        {
            try
            {
                var reportData = await _hrService.GeneratePaymentReport(year, month);

                return File(reportData, "text/csv", $"PaymentReport_{year}_{month:D2}.csv");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Dashboard");
            }
        }

        // Manage lecturers
        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _context.Lecturers.ToListAsync();
            return View(lecturers);
        }

        // Update lecturer details
        [HttpPost]
        public async Task<IActionResult> UpdateLecturer(Lecture lecturer)
        {
            try
            {
                _context.Lecturers.Update(lecturer);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Lecture details updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("ManageLecturers");
        }
    }
}
