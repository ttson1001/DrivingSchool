using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Dashboard;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<StudentProfile> _studentRepo;
        private readonly IRepository<InstructorProfile> _instructorRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Registration> _registrationRepo;
        private readonly IRepository<Transaction> _paymentRepo;

        public AdminService(
            IRepository<Account> accountRepo,
            IRepository<StudentProfile> studentRepo,
            IRepository<InstructorProfile> instructorRepo,
            IRepository<Course> courseRepo,
            IRepository<Registration> registrationRepo,
            IRepository<Transaction> paymentRepo)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
            _instructorRepo = instructorRepo;
            _courseRepo = courseRepo;
            _registrationRepo = registrationRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var now = DateTime.UtcNow;

            var totalAccounts = await _accountRepo.Get().CountAsync();
            var totalStudents = await _studentRepo.Get().CountAsync();
            var totalInstructors = await _instructorRepo.Get().CountAsync();
            var totalCourses = await _courseRepo.Get().CountAsync();
            var totalRegistrations = await _registrationRepo.Get().CountAsync();

            // --- Payment counts ---
            var topUpCount = await _paymentRepo.Get()
                .CountAsync(x => x.PaymentStatus == PaymentStatus.TopUp);

            var paidCount = await _paymentRepo.Get()
                .CountAsync(x => x.PaymentStatus == PaymentStatus.Paid);

            var refundCount = await _paymentRepo.Get()
                .CountAsync(x => x.PaymentStatus == PaymentStatus.Refund);

            var withdrawCount = await _paymentRepo.Get()
                .CountAsync(x => x.PaymentStatus == PaymentStatus.Withdraw);


            // --- Revenue calculation (Paid + TopUp - Refund - Withdraw) ---
            var totalRevenue = await _paymentRepo.Get()
                .SumAsync(x =>
                    x.PaymentStatus == PaymentStatus.Paid ? x.Amount ?? 0 :
                    x.PaymentStatus == PaymentStatus.TopUp ? x.Amount ?? 0 :
                    x.PaymentStatus == PaymentStatus.Refund ? -(x.Amount ?? 0) :
                    x.PaymentStatus == PaymentStatus.Withdraw ? -(x.Amount ?? 0) :
                    0
                );

            var monthlyRevenue = await _paymentRepo.Get()
                .Where(x => x.CreatedAt.Month == now.Month && x.CreatedAt.Year == now.Year)
                .SumAsync(x =>
                    x.PaymentStatus == PaymentStatus.Paid ? x.Amount ?? 0 :
                    x.PaymentStatus == PaymentStatus.TopUp ? x.Amount ?? 0 :
                    x.PaymentStatus == PaymentStatus.Refund ? -(x.Amount ?? 0) :
                    x.PaymentStatus == PaymentStatus.Withdraw ? -(x.Amount ?? 0) :
                    0
                );


            // Registration count per month (12 months)
            var registrationByMonth = await _registrationRepo.Get()
                .GroupBy(x => new { x.RegisterDate.Year, x.RegisterDate.Month })
                .Select(g => new RegistrationChartPoint
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToListAsync();


            return new AdminDashboardDto
            {
                TotalAccounts = totalAccounts,
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalCourses = totalCourses,
                TotalRegistrations = totalRegistrations,

                TopUpCount = topUpCount,
                PaidCount = paidCount,
                RefundCount = refundCount,
                WithdrawCount = withdrawCount,

                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                RegistrationPerMonth = registrationByMonth
            };
        }
    }
}
