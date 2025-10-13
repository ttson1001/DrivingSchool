using TutorDrive.Dtos.location;
using TutorDrive.Dtos.Location;

namespace TutorDrive.Services.IService
{
    public interface IProvinceSyncService
    {
        Task SyncProvincesAsync();
        Task SyncAllWardsAsync();
        Task<List<ProvinceExternalDto>> GetAllProvincesAsync();
        Task<List<WardDto>> GetWardsByProvinceCodeAsync(string provinceCode);
    }
}
