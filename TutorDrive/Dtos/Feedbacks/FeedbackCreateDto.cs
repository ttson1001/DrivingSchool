namespace TutorDrive.Dtos.Feedbacks
{
    public class FeedbackCreateDto
    {
        public long StudentProfileId { get; set; }
        public long? StaffId { get; set; }
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
        public long StudentProfileId { get; set; }
        public long? StaffId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
