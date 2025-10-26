namespace TutorDrive.Dtos.Staff
{
    public class TeacherCreateDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public string LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
