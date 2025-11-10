namespace TutorDrive.Dtos.LearningProgress
{
    public class LearningProgressItemDto
    {
        public long Id { get; set; }
        public long? SectionId { get; set; }
        public string? SectionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCompleted { get; set; }
        public long? InstructorId { get; set; }
        public string? InstructorName { get; set; }
        public string? Comment { get; set; }
    }
}
