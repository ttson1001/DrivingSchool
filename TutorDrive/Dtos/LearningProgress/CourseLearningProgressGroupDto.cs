namespace TutorDrive.Dtos.LearningProgress
{
    public class CourseLearningProgressGroupDto
    {
        public long? CourseId { get; set; }
        public string? CourseName { get; set; }
        public List<LearningProgressItemDto> Progresses { get; set; }
    }
}
