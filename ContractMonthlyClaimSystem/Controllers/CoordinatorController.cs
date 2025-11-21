using System.Data.Entity;
using ContractClaimSystem.Services;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractClaimSystem.Controllers
{
    [Authorize(Roles = "Coordinator,AcademicManager")]
    public class CoordinatorController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly ApplicationDbContext _context;

        public CoordinatorController(IClaimService claimService, ApplicationDbContext context)
        {
            _claimService = claimService;
            _context = context;
        }

        // View pending claims
        public async Task<IActionResult> PendingClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.Lecture)
                .Where(c => c.Status == "Pending Review" || c.Status == "Pending")
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        // Approve claim
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                var approvedBy = User.Identity.Name;
                await _claimService.ApproveClaim(claimId, approvedBy);

                TempData["Success"] = "Claim approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("PendingClaims");
        }

        // Reject claim
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId, string reason)
        {
            try
            {
                var rejectedBy = User.Identity.Name;
                await _claimService.RejectClaim(claimId, rejectedBy, reason);

                TempData["Success"] = "Claim rejected successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("PendingClaims");
        }

        // View all claims (with filtering)
        public async Task<IActionResult> AllClaims(string status = "All")
        {
            var query = _context.Claims.Include(c => c.Lecture).AsQueryable();

            if (status != "All")
            {
                query = query.Where(c => c.Status == status);
            }

            var claims = await query.OrderByDescending(c => c.SubmissionDate).ToListAsync();
            ViewBag.SelectedStatus = status;

            return View(claims);
        }
    }
}
