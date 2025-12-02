namespace TutorDrive.Dtos.Instructor
{
    public class InstructorDashboardDto
    {
        public long InstructorId { get; set; }
        public string InstructorName { get; set; }
        public string LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }

        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int ActiveSessions { get; set; }
        public int TotalStudents { get; set; }

        public double AverageRating { get; set; }
        public int TotalFeedback { get; set; }
        public List<FeedbackDto> RecentFeedbacks { get; set; }

        public int TotalVehicleUsage { get; set; }
        public DateTime? LastVehicleUsage { get; set; }
    }

    public class FeedbackDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StudentName { get; set; }
    }

}
