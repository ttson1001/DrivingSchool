using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.RegistrationExam
{
    public class RegistrationExamSearchRequest
    {
        public RegistrationStatus? Status { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
