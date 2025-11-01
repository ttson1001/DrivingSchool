using System;

namespace TutorDrive.Entities
{
    public class LearningProgress : IEntity
    {
        public long Id { get; set; }

        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public long? CourseId { get; set; }
        public Course Course { get; set; }

        public long? SectionId { get; set; }
        public Section Section { get; set; }

        public long? StaffId { get; set; }
        public Staff? Staff { get; set; }

        public bool IsCompleted { get; set; } = false;
        public string? Comment { get; set; }
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
