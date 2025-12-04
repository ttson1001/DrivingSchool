using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Exam;
using TutorDrive.Entities.Enum;

public class ExamResultDto
{
    public long Id { get; set; }
    public string ExamCode { get; set; }
    public ExamDto Exam { get; set; }

    public MeDto Student { get; set; }

    public float? TheoryScore { get; set; }
    public float? SimulationScore { get; set; }
    public float? TrackScore { get; set; }
    public float? RoadTestScore { get; set; }

    public ExamResultStatus Status { get; set; }
}
