using TutorDrive.Dtos.account;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Complaint
{
    public class ComplaintDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public string? Reply { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public MeDto Account { get; set; }
    }
}
