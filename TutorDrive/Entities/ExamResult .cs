using System.ComponentModel.DataAnnotations;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class ExamResult : IEntity
    {
        public long Id { get; set; }
        public long ExamId { get; set; }
        public Exam Exam { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }
        public string ExamCode { get; set; }
        public float? TheoryScore { get; set; }
        public float? SimulationScore { get; set; }
        public float? TrackScore { get; set; }  
        public float? RoadTestScore { get; set; }
        public ExamResultStatus Status { get; set; } = ExamResultStatus.Pending;
    }
}
