namespace TutorDrive.Dtos.LearningProgress
{
    public class AdminStudentProgressGroupDto
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentAvatar { get; set; }
        public string StudentEmail { get; set; }

        public long? InstructorId { get; set; }
        public string? InstructorName { get; set; }

        public List<LearningProgressItemDto> Progresses { get; set; }
    }
}
