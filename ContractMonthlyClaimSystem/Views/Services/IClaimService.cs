using System.Security.Claims;
using ContractMonthlyClaimSystem.ViewModels;
using ContractMonthlyClaimSystem.Views.ViewModels;

namespace ContractMonthlyClaimSystem.Services
{
    public interface IClaimService
    {
        Task<Claim> SubmitClaimAsync(SubmitClaimViewModel model, int lecturerId);
        Task<List<ClaimDetailsViewModel>> GetPendingClaimsAsync();
        Task<List<ClaimDetailsViewModel>> GetVerifiedClaimsAsync();
        Task<List<ClaimDetailsViewModel>> GetLecturerClaimsAsync(int lecturerId);
        Task<ClaimDetailsViewModel> GetClaimDetailsAsync(int claimId);
        Task<bool> VerifyClaimAsync(int claimId, int coordinatorId);
        Task<bool> RejectClaimAsync(int claimId, int coordinatorId, string reason);
        Task<bool> ApproveClaimAsync(int claimId, int managerId);
        Task<bool> RejectApprovalAsync(int claimId, int managerId, string reason);
    }
}