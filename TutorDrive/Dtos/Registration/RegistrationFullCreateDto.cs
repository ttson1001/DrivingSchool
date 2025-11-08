namespace TutorDrive.Dtos.Registration
{
    public class RegistrationFullCreateDto
    {
        public long CourseId { get; set; }
        public string? Note { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? CCCD { get; set; }
        public DateTime? DOB { get; set; }
        public string? Address { get; set; }

        public string CCCDFront { get; set; }
        public string CCCDBack { get; set; }

        public DateTime StartDateTime { get; set; }
        public List<string> StudyDays { get; set; } = new();
    }
}
