using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Dtos.common
{
    public class UploadImageRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }

}
