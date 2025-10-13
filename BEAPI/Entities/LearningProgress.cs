namespace TutorDrive.Entities
{
    public class LearningProgress : IEntity
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public long? CourseId { get; set; }
        public Course Course { get; set; }

        public int Comment { get; set; }
        public long? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
