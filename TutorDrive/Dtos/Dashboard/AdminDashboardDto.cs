namespace TutorDrive.Dtos.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalAccounts { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalRegistrations { get; set; }

        public decimal MonthlyRevenue { get; set; }
        public decimal TotalRevenue { get; set; }

        public int TopUpCount { get; set; }
        public int PaidCount { get; set; }
        public int RefundCount { get; set; }
        public int WithdrawCount { get; set; }

        public List<RegistrationChartPoint> RegistrationPerMonth { get; set; } = new();
    }

    public class RegistrationChartPoint
    {
        public string Month { get; set; } = "";
        public int Count { get; set; }
    }
}
