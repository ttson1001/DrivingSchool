namespace TutorDrive.Dtos.ExamResult
{
    public class ExamResultSearchDto
    {
        public string? Keyword { get; set; }
        public string? ExamCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
