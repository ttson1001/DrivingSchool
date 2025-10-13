using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Vehicle;

namespace TutorDrive.Services.IService
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetAllVehiclesAsync();
        Task CreateVehicleAsync(VehicleCreateDto dto);
        Task UpdateVehicleAsync(VehicleUpdateDto dto);
        Task<PagedResult<VehicleDto>> SearchVehiclesAsync(string? keyword, int page, int pageSize);
    }
}
