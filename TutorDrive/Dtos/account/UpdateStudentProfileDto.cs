using TutorDrive.Dtos.Address;

namespace TutorDrive.Dtos.account
{
    public class UpdateStudentProfileDto
    {
        public string? CMND { get; set; }
        public DateTime? DOB { get; set; }
        public string? Status { get; set; }
        public UpdateAddressDto? Address { get; set; }
    }
}
