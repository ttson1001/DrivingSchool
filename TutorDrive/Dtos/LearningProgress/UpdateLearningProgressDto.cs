namespace TutorDrive.Dtos.LearningProgress
{
    public class UpdateLearningProgressDto
    {
        public long Id { get; set; }
        public long? CourseId { get; set; }
        public long? SectionId { get; set; }
        public int Comment { get; set; }
    }

}
