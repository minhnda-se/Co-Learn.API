using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IS3StorageService
    {
        Task<string> GeneratePreSignedUploadUrlAsync(string fileKey, string contentType, int minutes = 10);
        string GetFileUrl(string fileKey, string folder = "public");
        Task<bool> DeleteFileAsync(string fileKey, string folder = "public");
        string GeneratePreSignedViewUrl(string fileKey, string folder = "public", int minutes = 10);
        Task<string> MoveFileAsync(string sourceKey, string destinationKey, string sourceFolder = "temp", string destinationFolder = "public");
        string GetBaseUrl();
    }
}
