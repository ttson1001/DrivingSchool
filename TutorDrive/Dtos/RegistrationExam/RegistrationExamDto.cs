using TutorDrive.Dtos.Course;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.RegistrationExam
{
    public class RegistrationExamDto
    {
        public long Id { get; set; }

        public long StudentProfileId { get; set; }
        public long AccountId { get; set; }

        public string StudentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string CccdFront { get; set; }
        public string CccdBack { get; set; }
        public string Avatar3x4 { get; set; }
        public string HealthCertificate { get; set; }
        public string ApplicationForm { get; set; }

        public RegistrationStatus Status { get; set; }
        public string? Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public CourseDto Course { get; set; }
    }

}
