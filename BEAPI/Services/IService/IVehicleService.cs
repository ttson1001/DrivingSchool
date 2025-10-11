using BEAPI.Dtos.Vehicle;

namespace BEAPI.Services.IService
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto?> CreateVehicleAsync(VehicleCreateDto dto);
        Task<VehicleDto?> UpdateVehicleAsync(VehicleUpdateDto dto);
    }
}
