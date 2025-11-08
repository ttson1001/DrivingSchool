namespace TutorDrive.Dtos.LearningProgress
{
    public class GenerateProgressDto
    {
        public long StudentId { get; set; }
        public long TeacherId { get; set; }
        public long CourseId { get; set; }
        public long RegisterId { get; set; }

        public DateTime StartDate { get; set; }
    }
}
