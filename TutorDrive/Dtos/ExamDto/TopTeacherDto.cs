using TutorDrive.Dtos.account;

namespace TutorDrive.Dtos.ExamDto
{
    public class TopTeacherDto
    {
        public long InstructorId { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
        public MeDto Instructor { get; set; }
    }
}
