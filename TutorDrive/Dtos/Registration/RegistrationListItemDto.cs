using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Registration
{
    public class RegistrationListItemDto
    {
        public long Id { get; set; }

        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }

        public string CCCD { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public long CourseId { get; set; }
        public string CourseName { get; set; }

        public RegistrationStatus Status { get; set; }
        public string? Note { get; set; }
        public DateTime RegisterDate { get; set; }

        public List<string>? FileUrls { get; set; }

        public DateTime StartDateTime { get; set; }
        public string StudyDays { get; set; }
        public decimal? Price { get; set; }
    }

}
