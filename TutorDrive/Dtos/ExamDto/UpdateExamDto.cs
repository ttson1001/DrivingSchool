namespace TutorDrive.Dtos.ExamDto
{
    public class UpdateExamDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }
}
