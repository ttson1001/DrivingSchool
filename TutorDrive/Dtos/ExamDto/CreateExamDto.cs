using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.ExamDto
{
    public class CreateExamDto
    {
        public string ExamCode { get; set; }
        public long CourseId { get; set; }
        public DateTime Date { get; set; }
        public ExamType Type { get; set; }
        public string Location { get; set; }
    }
}
