using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class RegistrationFile : IEntity
    {
        public long Id { get; set; }
        public long RegistrationId { get; set; }
        public Registration Registration { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
