using System.ComponentModel.DataAnnotations;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class RegistrationFile : IEntity
    {
        public long Id { get; set; }
        public long RegistrationId { get; set; }
        public Registration Registration { get; set; }
        public string Url { get; set; }
        public FileType FileType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
