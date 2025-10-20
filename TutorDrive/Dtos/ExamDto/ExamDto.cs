namespace TutorDrive.Dtos.ExamDto
{
    public class ExamDto
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }
}
