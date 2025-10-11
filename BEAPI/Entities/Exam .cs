using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class Exam : IEntity
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }

        public ICollection<ExamResult> Results { get; set; }
    }
}
