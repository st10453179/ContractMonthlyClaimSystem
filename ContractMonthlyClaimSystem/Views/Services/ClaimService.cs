using System.Security.Claims;
using ContractMonthlyClaimSystem.ViewModels;
using ContractMonthlyClaimSystem.Views.ViewModels;

namespace ContractMonthlyClaimSystem.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClaimService> _logger;

        public ClaimService(ApplicationDbContext context, ILogger<ClaimService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Claim> SubmitClaimAsync(SubmitClaimViewModel model, int lecturerId)
        {
            try
            {
                var claim = new Claims
                {
                    LecturerID = lecturerId,
                    Month = model.Month,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    AdditionalNotes = model.AdditionalNotes,
                    StatusID = 1, // Pending
                    SubmittedDate = DateTime.Now
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Claim {claim.ClaimID} submitted by lecturer {lecturerId}");
                return claim;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                throw new Exception("Failed to submit claim. Please try again.");
            }
        }

        public async Task<List<ClaimDetailsViewModel>> GetPendingClaimsAsync()
        {
            try
            {
                return await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.ClaimStatus)
                    .Include(c => c.SupportingDocuments)
                    .Where(c => c.StatusID == 1)
                    .Select(c => new ClaimDetailsViewModel
                    {
                        ClaimID = c.ClaimID,
                        LecturerName = c.Lecturer.FullName,
                        LecturerEmail = c.Lecturer.Email,
                        Department = c.Lecturer.Department,
                        Month = c.Month,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        TotalAmount = c.TotalAmount,
                        Status = c.ClaimStatus.StatusName,
                        StatusID = c.StatusID,
                        AdditionalNotes = c.AdditionalNotes,
                        SubmittedDate = c.SubmittedDate,
                        Documents = c.SupportingDocuments.ToList()
                    })
                    .OrderBy(c => c.SubmittedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending claims");
                throw new Exception("Failed to retrieve claims. Please try again.");
            }
        }

        public async Task<List<ClaimDetailsViewModel>> GetVerifiedClaimsAsync()
        {
            try
            {
                return await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.ClaimStatus)
                    .Include(c => c.SupportingDocuments)
                    .Where(c => c.StatusID == 2)
                    .Select(c => new ClaimDetailsViewModel
                    {
                        ClaimID = c.ClaimID,
                        LecturerName = c.Lecturer.FullName,
                        LecturerEmail = c.Lecturer.Email,
                        Department = c.Lecturer.Department,
                        Month = c.Month,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        TotalAmount = c.TotalAmount,
                        Status = c.ClaimStatus.StatusName,
                        StatusID = c.StatusID,
                        AdditionalNotes = c.AdditionalNotes,
                        SubmittedDate = c.SubmittedDate,
                        VerifiedDate = c.VerifiedDate,
                        Documents = c.SupportingDocuments.ToList()
                    })
                    .OrderBy(c => c.VerifiedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving verified claims");
                throw new Exception("Failed to retrieve claims. Please try again.");
            }
        }

        public async Task<List<ClaimDetailsViewModel>> GetLecturerClaimsAsync(int lecturerId)
        {
            try
            {
                return await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.ClaimStatus)
                    .Include(c => c.SupportingDocuments)
                    .Where(c => c.LecturerID == lecturerId)
                    .Select(c => new ClaimDetailsViewModel
                    {
                        ClaimID = c.ClaimID,
                        LecturerName = c.Lecturer.FullName,
                        LecturerEmail = c.Lecturer.Email,
                        Department = c.Lecturer.Department,
                        Month = c.Month,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        TotalAmount = c.TotalAmount,
                        Status = c.ClaimStatus.StatusName,
                        StatusID = c.StatusID,
                        AdditionalNotes = c.AdditionalNotes,
                        SubmittedDate = c.SubmittedDate,
                        VerifiedDate = c.VerifiedDate,
                        ApprovedDate = c.ApprovedDate,
                        RejectionReason = c.RejectionReason,
                        Documents = c.SupportingDocuments.ToList()
                    })
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lecturer claims");
                throw new Exception("Failed to retrieve your claims. Please try again.");
            }
        }

        public async Task<ClaimDetailsViewModel> GetClaimDetailsAsync(int claimId)
        {
            try
            {
                return await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.ClaimStatus)
                    .Include(c => c.SupportingDocuments)
                    .Where(c => c.ClaimID == claimId)
                    .Select(c => new ClaimDetailsViewModel
                    {
                        ClaimID = c.ClaimID,
                        LecturerName = c.Lecturer.FullName,
                        LecturerEmail = c.Lecturer.Email,
                        Department = c.Lecturer.Department,
                        Month = c.Month,
                        HoursWorked = c.HoursWorked,
                        HourlyRate = c.HourlyRate,
                        TotalAmount = c.TotalAmount,
                        Status = c.ClaimStatus.StatusName,
                        StatusID = c.StatusID,
                        AdditionalNotes = c.AdditionalNotes,
                        SubmittedDate = c.SubmittedDate,
                        VerifiedDate = c.VerifiedDate,
                        ApprovedDate = c.ApprovedDate,
                        RejectionReason = c.RejectionReason,
                        Documents = c.SupportingDocuments.ToList()
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving claim {claimId}");
                throw new Exception("Failed to retrieve claim details. Please try again.");
            }
        }

        public async Task<bool> VerifyClaimAsync(int claimId, int coordinatorId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null || claim.StatusID != 1)
                    return false;

                claim.StatusID = 2; // Verified
                claim.VerifiedDate = DateTime.Now;
                claim.VerifiedBy = coordinatorId;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Claim {claimId} verified by coordinator {coordinatorId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying claim {claimId}");
                throw new Exception("Failed to verify claim. Please try again.");
            }
        }

        public async Task<bool> RejectClaimAsync(int claimId, int coordinatorId, string reason)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null || claim.StatusID != 1)
                    return false;

                claim.StatusID = 4; // Rejected
                claim.RejectionReason = reason;
                claim.VerifiedBy = coordinatorId;
                claim.VerifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Claim {claimId} rejected by coordinator {coordinatorId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim {claimId}");
                throw new Exception("Failed to reject claim. Please try again.");
            }
        }

        public async Task<bool> ApproveClaimAsync(int claimId, int managerId)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null || claim.StatusID != 2)
                    return false;

                claim.StatusID = 3; // Approved
                claim.ApprovedDate = DateTime.Now;
                claim.ApprovedBy = managerId;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Claim {claimId} approved by manager {managerId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving claim {claimId}");
                throw new Exception("Failed to approve claim. Please try again.");
            }
        }

        public async Task<bool> RejectApprovalAsync(int claimId, int managerId, string reason)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim == null || claim.StatusID != 2)
                    return false;

                claim.StatusID = 4; // Rejected
                claim.RejectionReason = reason;
                claim.ApprovedBy = managerId;
                claim.ApprovedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Claim {claimId} rejected by manager {managerId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim {claimId}");
                throw new Exception("Failed to reject claim. Please try again.");
            }
        }

        Task<Claim> IClaimService.SubmitClaimAsync(SubmitClaimViewModel model, int lecturerId)
        {
            throw new NotImplementedException();
        }
    }
}
