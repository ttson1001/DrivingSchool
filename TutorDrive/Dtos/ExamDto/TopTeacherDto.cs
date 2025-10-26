namespace TutorDrive.Dtos.ExamDto
{
    public class TopTeacherDto
    {
        public long StaffId { get; set; }
        public string TeacherName { get; set; }
        public string Email { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
    }
}
