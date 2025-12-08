namespace TutorDrive.Dtos.LearningProgress
{
    public class CompletedCourseDto
    {
        public long? CourseId { get; set; }
        public string CourseName { get; set; }

        public long TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }

        public DateTimeOffset CompletedDate { get; set; }
        public int TotalSections { get; set; }
    }

}
