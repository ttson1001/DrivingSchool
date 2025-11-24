namespace TutorDrive.Dtos.ExamResult
{
    public class ExamResultImportDto
    {
        public string ExamCode { get; set; }
        public string StudentEmail { get; set; }
        public float? TheoryScore { get; set; }
        public float? SimulationScore { get; set; }
        public float? TrackScore { get; set; }
        public float? RoadTestScore { get; set; }
        public string Status { get; set; }
    }

}
