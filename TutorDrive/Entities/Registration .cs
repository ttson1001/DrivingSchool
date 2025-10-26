using System.ComponentModel.DataAnnotations;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Registration : IEntity
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        public RegistrationStatus Status { get; set; }
        public string? Note { get; set; }
        public ICollection<RegistrationFile> Files { get; set; }
    }
}
