namespace TutorDrive.Dtos.account
{
    public class UpdateInstructorProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }

        public string? LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Description { get; set; }
    }

}
