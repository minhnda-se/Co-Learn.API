using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CoLearn.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CoLearn.Infrastructure.Services
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;

        public S3StorageService(IConfiguration config)
        {
            _bucketName = config["AWS:BucketName"]
                ?? throw new ArgumentNullException("BucketName is missing in config");

            _region = config["AWS:Region"] ?? "ap-southeast-2";

            var accessKey = config["AWS:AccessKey"]
                ?? throw new ArgumentNullException("AWS AccessKey is missing in config");
            var secretKey = config["AWS:SecretKey"]
                ?? throw new ArgumentNullException("AWS SecretKey is missing in config");

            var regionEndpoint = RegionEndpoint.GetBySystemName(_region);

            _s3Client = new AmazonS3Client(accessKey, secretKey, new AmazonS3Config
            {
                RegionEndpoint = regionEndpoint
            });
        }

        /// <summary>
        /// Tạo URL upload cho file, mặc định lưu vào folder "temp/"
        /// </summary>
        public async Task<string> GeneratePreSignedUploadUrlAsync(string fileKey, string contentType, int minutes = 10)
        {
            string keyWithFolder = $"temp/{fileKey}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = keyWithFolder,
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                Verb = HttpVerb.PUT,
                ContentType = contentType
            };

            return _s3Client.GetPreSignedURL(request);
        }

        /// <summary>
        /// Lấy URL public của file trong folder cụ thể
        /// </summary>
        public string GetFileUrl(string fileKey, string folder = "public")
        {
            var encodedKey = Uri.EscapeDataString(fileKey).Replace("+", "%20");
            return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{folder}/{encodedKey}";
        }

        /// <summary>
        /// Xoá file khỏi bucket
        /// </summary>
        public async Task<bool> DeleteFileAsync(string fileKey, string folder = "public")
        {
            string keyWithFolder = $"{folder}/{fileKey}";
            var response = await _s3Client.DeleteObjectAsync(_bucketName, keyWithFolder);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Tạo presigned URL để xem/download file
        /// </summary>
        public string GeneratePreSignedViewUrl(string fileKey, string folder = "public", int minutes = 10)
        {
            string keyWithFolder = $"{folder}/{fileKey}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = keyWithFolder,
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                Verb = HttpVerb.GET
            };
            return _s3Client.GetPreSignedURL(request);
        }

        /// <summary>
        /// Di chuyển file từ folder này sang folder khác trong bucket
        /// </summary>
        public async Task<string> MoveFileAsync(string sourceKey, string destinationKey, string sourceFolder = "temp", string destinationFolder = "public")
        {
            string sourceFullKey = $"{sourceFolder}/{sourceKey}";
            string destinationFullKey = $"{destinationFolder}/{destinationKey}";

            // Copy file
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                SourceKey = sourceFullKey,
                DestinationBucket = _bucketName,
                DestinationKey = destinationFullKey
            };
            await _s3Client.CopyObjectAsync(copyRequest);

            // Delete file gốc
            await _s3Client.DeleteObjectAsync(_bucketName, sourceFullKey);

            return GetFileUrl(destinationKey, destinationFolder);
        }

        /// <summary>
        /// Lấy base URL bucket
        /// </summary>
        public string GetBaseUrl()
        {
            return $"https://{_bucketName}.s3.{_region}.amazonaws.com/";
        }
    }
}
