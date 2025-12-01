namespace TutorDrive.Dtos.Staff
{
    namespace TutorDrive.Dtos.Accounts
    {
        public class AccountCreateDto
        {
            public string Email { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Avartar { get; set; }
            public long RoleId { get; set; }
            public string? LicenseNumber { get; set; }
            public int? ExperienceYears { get; set; }
        }
    }

}
