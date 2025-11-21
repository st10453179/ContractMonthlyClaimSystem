using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContractClaimSystem.Services;
using ContractMonthlyClaimSystem.Data;
using System.Data.Entity;
using ContractMonthlyClaimSystem.Models;

namespace ContractClaimSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly ApplicationDbContext _context;

        public LecturerController(IClaimService claimService, ApplicationDbContext context)
        {
            _claimService = claimService;
            _context = context;
        }

        // GET: Submit Claim Page
        public IActionResult SubmitClaim()
        {
            return View();
        }

        // POST: Submit Claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(Claim claim)
        {
            try
            {
                // Get current lecturer ID (from logged-in user)
                claim.LecturerId = GetCurrentLecturerId();

                var submittedClaim = await _claimService.SubmitClaim(claim);

                TempData["Success"] = $"Claim submitted successfully! Status: {submittedClaim.Status}";
                return RedirectToAction("MyClaims");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(claim);
            }
        }

        // AJAX: Calculate payment (for real-time calculation)
        [HttpPost]
        public JsonResult CalculatePayment(decimal hoursWorked, decimal hourlyRate)
        {
            var total = _claimService.CalculateTotalPayment(hoursWorked, hourlyRate);
            return Json(new { totalPayment = total });
        }

        // View lecturer's claims
        public async Task<IActionResult> MyClaims()
        {
            var lecturerId = GetCurrentLecturerId();
            var claims = await _context.Claims
                .Where(c => c.LecturerId == lecturerId)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        private int GetCurrentLecturerId()
        {
            // Get from logged-in user's claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}
