using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Dtos.LearningProgress
{
    public class LearningProgressUpdateDto
    {
        public long Id { get; set; }
        public bool IsCompleted { get; set; }

        public string? Comment { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
