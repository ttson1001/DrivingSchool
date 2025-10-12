using BEAPI.Dtos.Vehicle;

namespace BEAPI.Services.IService
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetAllVehiclesAsync();
        Task CreateVehicleAsync(VehicleCreateDto dto);
        Task UpdateVehicleAsync(VehicleUpdateDto dto);
    }
}
