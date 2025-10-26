namespace TutorDrive.Dtos.LearningProgress
{
    public class LearningProgressDto
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public long? CourseId { get; set; }
        public string CourseName { get; set; }
        public long? SectionId { get; set; }
        public string SectionName { get; set; }
        public int Comment { get; set; }
    }
}
