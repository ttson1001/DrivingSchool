using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.RegistrationExam
{
    public class RegistrationExamStatusUpdateDto
    {
        public long Id { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? Comment { get; set; }
    }
}
