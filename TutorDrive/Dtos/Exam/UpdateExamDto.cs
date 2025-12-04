using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Exam
{
    public class UpdateExamDto
    {
        public long Id { get; set; }
        public string ExamCode { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public bool Theory { get; set; }
        public bool Simulation { get; set; }
        public bool Track { get; set; }
        public bool RoadTest { get; set; }
    }
}
