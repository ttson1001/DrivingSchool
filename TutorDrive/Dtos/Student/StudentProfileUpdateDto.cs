namespace TutorDrive.Dtos.Student
{
    public class StudentProfileUpdateDto
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? CMND { get; set; }
        public DateTime? DOB { get; set; }
        public string? Status { get; set; }
        public long? AddressId { get; set; }
    }
}
