using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class RegistrationExam: IEntity
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }
        public string CccdFront { get; set; }
        public string CccdBack { get; set; }
        public Course? Course { get; set; }
        public long? CourseId { get; set; }
        public string Avatar3x4 { get; set; }
        public string HealthCertificate { get; set; }
        public string ApplicationForm { get; set; }
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
        public string? Comment { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Exam? Exam { get; set; }
        public long? ExamId { get; set; }
    }
}
