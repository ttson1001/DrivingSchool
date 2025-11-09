using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;

namespace TutorDrive.Dtos.account
{
    public class MeDto
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string? PhoneNumber { get; set; }

        // Staff
        public string? LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }

        // Student
        public string? CMND { get; set; }
        public DateTime? DOB { get; set; }
        public AddressDto? Address { get; set; }
        public string? Status { get; set; }
    }
}
