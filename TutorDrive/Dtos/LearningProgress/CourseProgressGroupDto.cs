namespace TutorDrive.Dtos.LearningProgress
{
    public class CourseProgressGroupDto
    {
        public long CourseId { get; set; }
        public string CourseName { get; set; }

        public List<AdminStudentProgressGroupDto> Students { get; set; }
    }
}
