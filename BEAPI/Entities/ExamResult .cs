using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Entities
{
    public class ExamResult : IEntity
    {
        public long Id { get; set; }
        public long ExamId { get; set; }
        public Exam Exam { get; set; }

        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public decimal? Score { get; set; }
        public string Status { get; set; }
    }
}
