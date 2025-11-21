using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;

namespace ContractClaimSystem.Services
{
    public interface IClaimService
    {
        decimal CalculateTotalPayment(decimal hoursWorked, decimal hourlyRate);
        bool ValidateClaim(Claim claim, out List<string> errors);
        Task<bool> AutoVerifyClaim(Claim claim);
        Task<Claim> SubmitClaim(Claim claim);
        Task<Claim> ApproveClaim(int claimId, string approvedBy);
        Task<Claim> RejectClaim(int claimId, string rejectedBy, string reason);
        Task SubmitClaim(System.Security.Claims.Claim claim);
    }

    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;

        public ClaimService(ApplicationDbContext context)
        {
            _context = context;
        }

        // AUTOMATION 1: Auto-calculate payment
        public decimal CalculateTotalPayment(decimal hoursWorked, decimal hourlyRate)
        {
            return hoursWorked * hourlyRate;
        }

        // AUTOMATION 2: Validate claim data
        public bool ValidateClaim(Claim claim, out List<string> errors)
        {
            errors = new List<string>();

            // Check hours worked
            if (claim.HoursWorked <= 0)
            {
                errors.Add("Hours worked must be greater than 0");
            }
            if (claim.HoursWorked > 744) // Max hours in a month (31 days * 24 hours)
            {
                errors.Add("Hours worked exceeds maximum allowed (744 hours per month)");
            }

            // Check hourly rate
            if (claim.HourlyRate < 100)
            {
                errors.Add("Hourly rate must be at least R100");
            }
            if (claim.HourlyRate > 5000)
            {
                errors.Add("Hourly rate exceeds maximum allowed (R5000)");
            }

            // Check if total payment is correctly calculated
            var expectedTotal = CalculateTotalPayment(claim.HoursWorked, claim.HourlyRate);
            if (Math.Abs(claim.TotalPayment - expectedTotal) > 0.01m)
            {
                errors.Add("Total payment calculation is incorrect");
            }

            return errors.Count == 0;
        }

        // AUTOMATION 3: Auto-verify claim against predefined criteria
        public async Task<bool> AutoVerifyClaim(Claim claim)
        {
            // Define verification rules
            var maxHoursPerMonth = 160; // Standard full-time hours
            var maxHourlyRate = 3000;
            var minHourlyRate = 150;

            // Auto-approve if within standard parameters
            if (claim.HoursWorked <= maxHoursPerMonth &&
                claim.HourlyRate >= minHourlyRate &&
                claim.HourlyRate <= maxHourlyRate)
            {
                return true; // Auto-approve
            }

            return false; // Needs manual review
        }

        // Submit claim
        public async Task<Claim> SubmitClaim(Claim claim)
        {
            // Auto-calculate payment
            claim.TotalPayment = CalculateTotalPayment(claim.HoursWorked, claim.HourlyRate);
            claim.SubmissionDate = DateTime.Now;

            // Validate
            if (!ValidateClaim(claim, out List<string> errors))
            {
                throw new Exception("Validation failed: " + string.Join(", ", errors));
            }

            // Auto-verify
            bool autoApproved = await AutoVerifyClaim(claim);
            claim.Status = autoApproved ? "Auto-Approved" : "Pending Review";

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        // Approve claim
        public async Task<Claim> ApproveClaim(int claimId, string approvedBy)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                throw new Exception("Claim not found");
            }

            claim.Status = "Approved";
            claim.ApprovalDate = DateTime.Now;
            claim.ApprovedBy = approvedBy;

            await _context.SaveChangesAsync();
            return claim;
        }

        // Reject claim
        public async Task<Claim> RejectClaim(int claimId, string rejectedBy, string reason)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null)
            {
                throw new Exception("Claim not found");
            }

            claim.Status = "Rejected";
            claim.ApprovalDate = DateTime.Now;
            claim.ApprovedBy = rejectedBy;
            claim.RejectionReason = reason;

            await _context.SaveChangesAsync();
            return claim;
        }
    }
}