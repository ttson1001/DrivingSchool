using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TutorDrive.Services.IService
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
