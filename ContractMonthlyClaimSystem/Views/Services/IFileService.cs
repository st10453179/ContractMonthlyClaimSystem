using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services
{
    public interface IFileService
    {
        Task<SupportingDocument> UploadDocumentAsync(IFormFile file, int claimId);
        Task<List<SupportingDocument>> UploadMultipleDocumentsAsync(List<IFormFile> files, int claimId);
        Task<bool> DeleteDocumentAsync(int documentId);
        Task<byte[]> DownloadDocumentAsync(int documentId);
        bool ValidateFile(IFormFile file);
    }
}
