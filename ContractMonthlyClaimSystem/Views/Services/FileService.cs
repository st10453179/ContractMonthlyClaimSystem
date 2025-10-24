using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx", ".doc", ".xls" };

        public FileService(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        public async Task<SupportingDocument> UploadDocumentAsync(IFormFile file, int claimId)
        {
            try
            {
                if (!ValidateFile(file))
                    throw new Exception("Invalid file. Please upload a PDF, DOCX, or XLSX file under 5MB.");

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "claims");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new SupportingDocument
                {
                    ClaimID = claimId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/claims/{uniqueFileName}",
                    UploadedAt = DateTime.Now
                };

                _context.SupportingDocuments.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Document uploaded for claim {claimId}: {file.FileName}");
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                throw new Exception("Failed to upload document. Please try again.");
            }
        }

        public async Task<List<SupportingDocument>> UploadMultipleDocumentsAsync(List<IFormFile> files, int claimId)
        {
            var documents = new List<SupportingDocument>();

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var document = await UploadDocumentAsync(file, claimId);
                    documents.Add(document);
                }
            }

            return documents;
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            try
            {
                var document = await _context.SupportingDocuments.FindAsync(documentId);
                if (document == null)
                    return false;

                var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _context.SupportingDocuments.Remove(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Document {documentId} deleted");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting document {documentId}");
                throw new Exception("Failed to delete document. Please try again.");
            }
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId)
        {
            try
            {
                var document = await _context.SupportingDocuments.FindAsync(documentId);
                if (document == null)
                    throw new Exception("Document not found");

                var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (!File.Exists(filePath))
                    throw new Exception("File not found on server");

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document {documentId}");
                throw new Exception("Failed to download document. Please try again.");
            }
        }

        Task<SupportingDocument> IFileService.UploadDocumentAsync(IFormFile file, int claimId)
        {
            throw new NotImplementedException();
        }

        Task<List<SupportingDocument>> IFileService.UploadMultipleDocumentsAsync(List<IFormFile> files, int claimId)
        {
            throw new NotImplementedException();
        }
    }
}