using BEAPI.Dtos.Vehicle;

namespace BEAPI.Services.IService
{
    public interface IVehicleUsageHistoryService
    {
        Task<List<VehicleUsageHistoryDto>> GetAllAsync();
        Task<VehicleUsageHistoryDto> GetByIdAsync(long id);
        Task<List<VehicleUsageHistoryDto>> GetAllByVehicleIdAsync(long vehicleId);
        Task<List<VehicleUsageHistoryDto>> GetAllByAccountIdAsync(long accountId);
        Task CreateAsync(VehicleUsageHistoryCreateDto dto);
        Task UpdateAsync(VehicleUsageHistoryUpdateDto dto);
    }
}
