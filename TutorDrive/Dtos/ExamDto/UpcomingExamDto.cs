using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.ExamDto
{
    public class UpcomingExamDto
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public string ExamCode { get; set; }

        public ExamType Type { get; set; }
        public string TypeName { get; set; }

        public DateTime ExamDate { get; set; }
        public string Location { get; set; }
    }

}
