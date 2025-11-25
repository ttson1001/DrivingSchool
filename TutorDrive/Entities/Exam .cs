using System.ComponentModel.DataAnnotations;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Exam : IEntity
    {
        public long Id { get; set; }
        public string ExamCode { get; set; }
        public long CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime ExamDate { get; set; }
        public bool Theory { get; set; }
        public bool Simulation { get; set; }
        public bool Track { get; set; }
        public bool RoadTest { get; set; }
        public string Location { get; set; }
        public ICollection<ExamResult> Results { get; set; }
    }

}
