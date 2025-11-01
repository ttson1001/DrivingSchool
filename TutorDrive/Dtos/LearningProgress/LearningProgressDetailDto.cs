namespace TutorDrive.Dtos.LearningProgress
{
    namespace TutorDrive.Dtos.LearningProgress
    {
        public class LearningProgressDetailDto
        {
            public long Id { get; set; }

            public long? StudentId { get; set; }
            public string? StudentName { get; set; }
            public string? StudentEmail { get; set; }

            public long? CourseId { get; set; }
            public string? CourseName { get; set; }

            public long? SectionId { get; set; }
            public string? SectionTitle { get; set; }

            public long? TeacherId { get; set; }
            public string? TeacherName { get; set; }
            public string? TeacherEmail { get; set; }

            public bool IsCompleted { get; set; }
            public string? Comment { get; set; }

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? LastUpdated { get; set; }
        }
    }

}
