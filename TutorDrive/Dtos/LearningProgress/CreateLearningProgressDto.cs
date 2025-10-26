namespace TutorDrive.Dtos.LearningProgress
{
    public class CreateLearningProgressDto
    {
        public long StudentProfileId { get; set; }
        public long? CourseId { get; set; }
        public long? SectionId { get; set; }
        public int Comment { get; set; }
    }
}
