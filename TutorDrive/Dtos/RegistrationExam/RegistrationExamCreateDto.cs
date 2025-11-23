namespace TutorDrive.Dtos.RegistrationExam
{
    public class RegistrationExamCreateDto
    {

        public long CourseId { get; set; }
        public string CccdFront { get; set; }

        public string CccdBack { get; set; }

        public string Avatar3x4 { get; set; }

        public string HealthCertificate { get; set; }

        public string ApplicationForm { get; set; }
    }
}
