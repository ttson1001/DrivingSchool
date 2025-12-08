
using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Account : IEntity
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public string? ResetOtp { get; set; }
        public DateTimeOffset? ResetOtpExpiry { get; set; }
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public StudentProfile? StudentProfile { get; set; }
        public InstructorProfile? InstructorProfile { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
