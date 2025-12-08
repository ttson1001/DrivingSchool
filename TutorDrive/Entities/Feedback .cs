namespace TutorDrive.Entities
{
    public class Feedback : IEntity
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public long? InstructorProfileId { get; set; }
        public InstructorProfile InstructorProfile { get; set; }

        public long? CourseId { get; set; }
        public Course Course { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
