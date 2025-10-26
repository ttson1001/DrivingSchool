using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Registration
{
    public class RegistrationSearchDto
    {
        public string? Keyword { get; set; }          
        public RegistrationStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }         
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
