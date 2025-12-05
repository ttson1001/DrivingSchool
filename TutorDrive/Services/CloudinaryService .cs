using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TutorDrive.Extension.Cloudary;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Không có file để upload.");

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "tutordrive_uploads/images"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return uploadResult.SecureUrl.AbsoluteUri;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "default")
        {
            if (file == null || file.Length == 0)
                throw new Exception("Không có file để upload.");

            long maxSize = 100 * 1024 * 1024;

            if (file.Length > maxSize)
                throw new Exception("Dung lượng file vượt quá giới hạn 100MB.");

            var allowedExtensions = new[]
            {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".png", ".jpg", ".jpeg"
    };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new Exception($"Định dạng file {ext} không được hỗ trợ.");

            using var httpClient = new HttpClient();
            var requestContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            requestContent.Add(fileContent, "file", file.FileName);

            var uploadApiUrl = $"http://103.200.20.45:3030/upload/{folder}";
            var response = await httpClient.PostAsync(uploadApiUrl, requestContent);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Upload thất bại: {response.StatusCode}");

            var resultJson = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(resultJson);
            var url = json.RootElement.GetProperty("file").GetProperty("url").GetString();

            return url!;
        }

    }
}
