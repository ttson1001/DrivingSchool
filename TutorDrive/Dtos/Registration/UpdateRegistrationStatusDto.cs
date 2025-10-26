using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Registration
{
    public class UpdateRegistrationStatusDto
    {
        public long RegistrationId { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
