using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Complaint : IEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? Reply { get; set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Account Account { get; set; }
        public string? Url { get; set; }
    }
}
