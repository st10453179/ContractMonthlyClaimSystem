
using System.Security.Claims;
using ContractMonthlyClaimSystem.Services;
using ContractMonthlyClaimSystem.ViewModels;
using ContractMonthlyClaimSystem.Views.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize]
    public class ClaimController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(
            IClaimService claimService,
            IFileService fileService,
            ApplicationDbContext context,
            ILogger<ClaimController> logger)
        {
            _claimService = claimService;
            _fileService = fileService;
            _context = context;
            _logger = logger;
        }

        // ====================
        // LECTURER ACTIONS
        // ====================

        [Authorize(Policy = "LecturerOnly")]
        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [Authorize(Policy = "LecturerOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitClaimViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validate main document
                if (model.SupportingDocument == null || !_fileService.ValidateFile(model.SupportingDocument))
                {
                    ModelState.AddModelError("SupportingDocument", "Please upload a valid document (PDF, DOCX, XLSX, max 5MB)");
                    return View(model);
                }

                // Get lecturer ID from claims
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.Email == userEmail);

                if (lecturer == null)
                {
                    TempData["Error"] = "Lecturer profile not found. Please contact support.";
                    return View(model);
                }

                // Submit claim
                var claim = await _claimService.SubmitClaimAsync(model, lecturer.LecturerID);

                // Upload supporting document
                await _fileService.UploadDocumentAsync(model.SupportingDocument, claim.ClaimID);

                // Upload additional documents if any
                if (model.AdditionalDocuments != null && model.AdditionalDocuments.Any())
                {
                    var validDocuments = model.AdditionalDocuments.Where(d => _fileService.ValidateFile(d)).ToList();
                    if (validDocuments.Any())
                    {
                        await _fileService.UploadMultipleDocumentsAsync(validDocuments, claim.ClaimID);
                    }
                }

                TempData["Success"] = $"Claim submitted successfully! Your claim ID is {claim.ClaimID}. Total Amount: R {claim.TotalAmount:N2}";
                return RedirectToAction("Track");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [Authorize(Policy = "LecturerOnly")]
        [HttpGet]
        public async Task<IActionResult> Track()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.Email == userEmail);

                if (lecturer == null)
                {
                    TempData["Error"] = "Lecturer profile not found.";
                    return View(new List<ClaimDetailsViewModel>());
                }

                var claims = await _claimService.GetLecturerClaimsAsync(lecturer.LecturerID);
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims");
                TempData["Error"] = ex.Message;
                return View(new List<ClaimDetailsViewModel>());
            }
        }

        // ====================
        // COORDINATOR ACTIONS
        // ====================

        [Authorize(Policy = "CoordinatorOnly")]
        [HttpGet]
        public async Task<IActionResult> Review()
        {
            try
            {
                var claims = await _claimService.GetPendingClaimsAsync();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending claims");
                TempData["Error"] = ex.Message;
                return View(new List<ClaimDetailsViewModel>());
            }
        }

        [Authorize(Policy = "CoordinatorOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int claimId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _claimService.VerifyClaimAsync(claimId, userId);

                if (result)
                {
                    TempData["Success"] = "Claim verified successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to verify claim. It may have already been processed.";
                }

                return RedirectToAction("Review");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying claim {claimId}");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Review");
            }
        }

        [Authorize(Policy = "CoordinatorOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int claimId, string rejectionReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rejectionReason))
                {
                    TempData["Error"] = "Please provide a reason for rejection.";
                    return RedirectToAction("Review");
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _claimService.RejectClaimAsync(claimId, userId, rejectionReason);

                if (result)
                {
                    TempData["Success"] = "Claim rejected successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to reject claim. It may have already been processed.";
                }

                return RedirectToAction("Review");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim {claimId}");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Review");
            }
        }

        // ====================
        // MANAGER ACTIONS
        // ====================

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet]
        public async Task<IActionResult> Approve()
        {
            try
            {
                var claims = await _claimService.GetVerifiedClaimsAsync();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving verified claims");
                TempData["Error"] = ex.Message;
                return View(new List<ClaimDetailsViewModel>());
            }
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _claimService.ApproveClaimAsync(claimId, userId);

                if (result)
                {
                    TempData["Success"] = "Claim approved successfully! Payment processing will begin.";
                }
                else
                {
                    TempData["Error"] = "Failed to approve claim. It may have already been processed.";
                }

                return RedirectToAction("Approve");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving claim {claimId}");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Approve");
            }
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApproval(int claimId, string rejectionReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rejectionReason))
                {
                    TempData["Error"] = "Please provide a reason for rejection.";
                    return RedirectToAction("Approve");
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _claimService.RejectApprovalAsync(claimId, userId, rejectionReason);

                if (result)
                {
                    TempData["Success"] = "Claim rejected successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to reject claim. It may have already been processed.";
                }

                return RedirectToAction("Approve");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim {claimId}");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Approve");
            }
        }

        // ====================
        // SHARED ACTIONS
        // ====================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var claim = await _claimService.GetClaimDetailsAsync(id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check authorization
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "Lecturer")
                {
                    if (claim.LecturerEmail != userEmail)
                    {
                        TempData["Error"] = "You are not authorized to view this claim.";
                        return RedirectToAction("Track");
                    }
                }

                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving claim details {id}");
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                var document = await _context.SupportingDocuments
                    .Include(d => d.Claim)
                    .FirstOrDefaultAsync(d => d.DocumentID == id);

                if (document == null)
                {
                    TempData["Error"] = "Document not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check authorization
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "Lecturer")
                {
                    var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.Email == userEmail


}         