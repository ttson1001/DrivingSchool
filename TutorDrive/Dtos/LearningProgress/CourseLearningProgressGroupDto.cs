using TutorDrive.Dtos.Feedbacks;

namespace TutorDrive.Dtos.LearningProgress
{
    public class CourseLearningProgressGroupDto
    {
        public long? CourseId { get; set; }
        public string? CourseName { get; set; }
        public List<LearningProgressItemDto> Progresses { get; set; }
        public FeedbackDto Feedback { get; set; }
        public bool? IsRegistrationExam { get; set; }
    }
}
