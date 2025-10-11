using BEAPI.Dtos.location;
using BEAPI.Dtos.Location;

namespace BEAPI.Services.IService
{
    public interface IProvinceSyncService
    {
        Task SyncProvincesAsync();
        Task SyncAllWardsAsync();
        Task<List<ProvinceExternalDto>> GetAllProvincesAsync();
        Task<List<WardDto>> GetWardsByProvinceCodeAsync(string provinceCode);
    }
}
