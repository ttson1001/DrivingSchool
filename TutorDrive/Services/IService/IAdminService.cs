using TutorDrive.Dtos.Dashboard;

namespace TutorDrive.Services.IService
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}
