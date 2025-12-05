using TutorDrive.Dtos.Address;

namespace TutorDrive.Dtos.account
{
    public class UpdateStudentProfileDto
    {
        public string? CMND { get; set; }
        public DateTimeOffset? DOB { get; set; }
        public UpdateAddressDto? Address { get; set; }
        public string? Avatar { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
