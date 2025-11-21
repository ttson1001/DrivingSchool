using TutorDrive.Dtos.Vehicle;

namespace TutorDrive.Services.IService
{
    public interface IVehicleUsageHistoryService
    {
        Task<List<VehicleUsageHistoryDto>> GetAllAsync();
        Task<VehicleUsageHistoryDto> GetByIdAsync(long id);
        Task<List<VehicleUsageHistoryDto>> GetAllByVehicleIdAsync(long vehicleId);
        Task<List<VehicleUsageHistoryDto>> GetAllByAccountIdAsync(long accountId);
        Task CreateAsync(long accountId, VehicleUsageHistoryCreateDto dto);
        Task UpdateAsync(VehicleUsageHistoryUpdateDto dto);
    }
}
