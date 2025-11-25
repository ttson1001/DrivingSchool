using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.ExamDto
{
    public class CreateExamDto
    {
        public string ExamCode { get; set; }
        public long CourseId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public bool Theory { get; set; }
        public bool Simulation { get; set; }
        public bool Track { get; set; }
        public bool RoadTest { get; set; }
    }
}
