using CoLearn.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoLearn.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IS3StorageService _s3StorageService;

        public FilesController(IS3StorageService s3StorageService)
        {
            _s3StorageService = s3StorageService;
        }

        [HttpPost("presigned")]
        [Authorize] // Bắt buộc login
        public async Task<IActionResult> GeneratePresignedUrl([FromBody] FileRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.ContentType))
                return BadRequest("FileName và ContentType là bắt buộc.");

            // Lấy userId từ token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!UserHasAccess(userId, request.Folder))
                return Forbid("Bạn không có quyền upload file vào folder này.");

            // Tạo fileKey
            string fileKey = $"{request.Folder}/{Guid.NewGuid()}_{request.FileName}";

            string presignedUrl = await _s3StorageService.GeneratePreSignedUploadUrlAsync(fileKey, request.ContentType, 10);

            return Ok(new FileResponseDto
            {
                FileKey = fileKey,
                PresignedUrl = presignedUrl,
                FileUrl = _s3StorageService.GetFileUrl(fileKey, request.Folder)
            });
        }

        [HttpDelete("{fileKey}")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(string fileKey)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!UserHasDeleteAccess(userId, fileKey))
                return Forbid("Bạn không có quyền xóa file này.");

            var success = await _s3StorageService.DeleteFileAsync(fileKey);
            if (!success) return NotFound("File không tồn tại hoặc đã bị xóa.");
            return NoContent();
        }

        [HttpGet("{fileKey}")]
        public IActionResult GetFileUrl(string fileKey)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(new { fileUrl = _s3StorageService.GetFileUrl(fileKey) });
        }

        [HttpPost("presigned/view")]
        [Authorize]
        public IActionResult GeneratePresignedViewUrl([FromBody] FileViewRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!UserHasAccessToView(userId, request.FileKey))
                return Forbid("Bạn không có quyền xem file này.");

            string presignedUrl = _s3StorageService.GeneratePreSignedViewUrl(request.FileKey, request.Folder, 10);

            return Ok(new { presignedUrl });
        }

        private bool UserHasAccess(string userId, string folder)
        {
            // Logic kiểm tra quyền upload file tùy folder
            if (folder == "private")
                return IsVipUser(userId);
            if (folder == "temp")
                return IsAuthenticated(userId);
            return true; // public folder
        }

        private bool UserHasDeleteAccess(string userId, string fileKey)
        {
            // Logic kiểm tra quyền xóa file
            return IsOwnerOfFile(userId, fileKey) || IsAdmin(userId);
        }

        private bool UserHasAccessToView(string userId, string fileKey)
        {
            if (fileKey.StartsWith("private/"))
                return IsVipUser(userId);
            return true;
        }

        private bool IsVipUser(string userId)
        {
            // TODO: kiểm tra trong DB xem user có VIP hay không
            return true;
        }

        private bool IsAuthenticated(string userId)
        {
            return !string.IsNullOrEmpty(userId);
        }

        private bool IsOwnerOfFile(string userId, string fileKey)
        {
            // TODO: kiểm tra trong DB xem fileKey thuộc userId không
            return true;
        }

        private bool IsAdmin(string userId)
        {
            // TODO: kiểm tra user role admin trong DB
            return false;
        }
    }

    public class FileRequestDto
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string Folder { get; set; } = "temp";
    }

    public class FileResponseDto
    {
        public string FileKey { get; set; } = null!;
        public string PresignedUrl { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
    }

    public class FileViewRequestDto
    {
        public string FileKey { get; set; } = null!;
        public string Folder { get; set; } = "public";
    }
}
