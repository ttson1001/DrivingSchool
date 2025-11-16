namespace TutorDrive.Dtos.LearningProgress
{
    public class StudentLearningProgressGroupDto
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }

        public List<LearningProgressItemDto> Progresses { get; set; }
    }

}
