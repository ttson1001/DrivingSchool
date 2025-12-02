using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Dashboard;
using TutorDrive.Dtos.Instructor;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class InstructorDashboardService : IInstructorDashboardService
    {
        private readonly IRepository<InstructorProfile> _instructorRepo;
        private readonly IRepository<LearningProgress> _progressRepo;
        private readonly IRepository<Feedback> _feedbackRepo;
        private readonly IRepository<VehicleUsageHistory> _usageRepo;

        public InstructorDashboardService(
            IRepository<InstructorProfile> instructorRepo,
            IRepository<LearningProgress> progressRepo,
            IRepository<Feedback> feedbackRepo,
            IRepository<VehicleUsageHistory> usageRepo)
        {
            _instructorRepo = instructorRepo;
            _progressRepo = progressRepo;
            _feedbackRepo = feedbackRepo;
            _usageRepo = usageRepo;
        }

        public async Task<InstructorDashboardDto> GetDashboardAsync(long accountId)
        {
            var instructor = await _instructorRepo.Get().Include(x => x.Account)
                .Where(x => x.AccountId == accountId)
                .FirstOrDefaultAsync();

            if (instructor == null)
                throw new Exception("Instructor not found");

            var instructorId = instructor.Id;

            var progressQuery = _progressRepo.Get()
                .Where(lp => lp.InstructorProfileId == instructorId);

            var totalSessions = await progressQuery.CountAsync();
            var completedSessions = await progressQuery.CountAsync(lp => lp.IsCompleted);
            var activeSessions = totalSessions - completedSessions;

            var totalStudents = await progressQuery
                .Select(lp => lp.StudentProfileId)
                .Distinct()
                .CountAsync();

            var feedbackQuery = _feedbackRepo.Get()
                .Where(f => f.InstructorProfileId == instructorId);

            var totalFeedback = await feedbackQuery.CountAsync();

            double avgRating = totalFeedback == 0
                ? 0
                : await feedbackQuery.AverageAsync(f => f.Rating);

            var recentFeedbacks = await feedbackQuery
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .Include(f => f.InstructorProfile).ThenInclude(x => x.Account)
                .Select(f => new FeedbackDto
                {
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,
                    StudentName = f.InstructorProfile.Account.FullName
                })
                .ToListAsync();

            var totalVehicleUsage = await _usageRepo.Get()
                .CountAsync(v => v.AccountId == accountId);

            var lastVehicleUsage = await _usageRepo.Get()
                .Where(v => v.AccountId == accountId)
                .OrderByDescending(v => v.StartTime)
                .Select(v => v.StartTime)
                .FirstOrDefaultAsync();

            return new InstructorDashboardDto
            {
                InstructorId = instructorId,
                InstructorName = instructor.Account.FullName,
                LicenseNumber = instructor.LicenseNumber,
                ExperienceYears = instructor.ExperienceYears,

                TotalSessions = totalSessions,
                CompletedSessions = completedSessions,
                ActiveSessions = activeSessions,
                TotalStudents = totalStudents,

                AverageRating = avgRating,
                TotalFeedback = totalFeedback,
                RecentFeedbacks = recentFeedbacks,

                TotalVehicleUsage = totalVehicleUsage,
                LastVehicleUsage = lastVehicleUsage
            };
        }
    }
}
