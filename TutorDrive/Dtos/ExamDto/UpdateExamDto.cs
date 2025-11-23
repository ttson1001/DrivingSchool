using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.ExamDto
{
    public class UpdateExamDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public ExamType Type { get; set; }
        public string Location { get; set; }
    }

}
