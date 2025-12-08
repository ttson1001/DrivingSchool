using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Exam
{
    public class UpcomingExamDto
    {
        public long Id { get; set; }
        public string ExamCode { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }

        public DateTimeOffset ExamDate { get; set; }

        public bool Theory { get; set; }
        public bool Simulation { get; set; }
        public bool Track { get; set; }
        public bool RoadTest { get; set; }

        public string Location { get; set; }
    }

}
