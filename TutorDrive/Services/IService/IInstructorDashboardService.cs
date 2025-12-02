using TutorDrive.Dtos.Instructor;

namespace TutorDrive.Services.IService
{
    public interface IInstructorDashboardService
    {
        Task<InstructorDashboardDto> GetDashboardAsync(long accountId);
    }
}
