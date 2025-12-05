using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TutorDrive.Services.IService
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadFileAsync(IFormFile file, string folder = "TUTOR");
    }
}
