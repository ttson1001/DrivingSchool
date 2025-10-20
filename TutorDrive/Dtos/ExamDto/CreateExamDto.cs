namespace TutorDrive.Dtos.ExamDto
{
    public class CreateExamDto
    {
        public long CourseId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }

}
