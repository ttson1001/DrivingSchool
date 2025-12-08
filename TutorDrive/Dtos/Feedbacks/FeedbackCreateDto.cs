using TutorDrive.Dtos.account;

namespace TutorDrive.Dtos.Feedbacks
{
    public class FeedbackCreateDto
    {
        public long CourseId { get; set; }
        public long? InstructorProfileId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class FeedbackUpdateDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class FeedbackDto
    {
        public long Id { get; set; }

        public MeDto Student { get; set; }
        public MeDto? Instructor { get; set; }

        public long? CourseId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

}
