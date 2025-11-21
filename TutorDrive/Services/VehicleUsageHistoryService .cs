using TutorDrive.Dtos.Vehicle;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace TutorDrive.Services
{
    public class VehicleUsageHistoryService : IVehicleUsageHistoryService
    {
        private readonly IRepository<VehicleUsageHistory> _historyRepo;
        private readonly IRepository<Vehicle> _vehicleRepo;
        private readonly IRepository<Account> _accountRepo;

        public VehicleUsageHistoryService(
            IRepository<VehicleUsageHistory> historyRepo,
            IRepository<Vehicle> vehicleRepo,
            IRepository<Account> accountRepo)
        {
            _historyRepo = historyRepo;
            _vehicleRepo = vehicleRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<VehicleUsageHistoryDto>> GetAllAsync()
        {
            var list = await _historyRepo.Get().ToListAsync();
            return list.Select(x => new VehicleUsageHistoryDto
            {
                Id = x.Id,
                VehicleId = x.VehicleId,
                AccountId = x.AccountId,
                StartTime = x.StartTime,
                EndTime = x.EndTime
            }).ToList();
        }

        public async Task<VehicleUsageHistoryDto> GetByIdAsync(long id)
        {
            var entity = await _historyRepo.Get().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) throw new Exception("Không tìm thấy");

            return new VehicleUsageHistoryDto
            {
                Id = entity.Id,
                VehicleId = entity.VehicleId,
                AccountId = entity.AccountId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        public async Task<List<VehicleUsageHistoryDto>> GetAllByVehicleIdAsync(long vehicleId)
        {
            return await _historyRepo.Get().Where(x => x.VehicleId == vehicleId).Select(x => new VehicleUsageHistoryDto
            {
                Id = x.Id,
                VehicleId = x.VehicleId,
                AccountId = x.AccountId,
                StartTime = x.StartTime,
                EndTime = x.EndTime
            }).ToListAsync();
        }

        public async Task<List<VehicleUsageHistoryDto>> GetAllByAccountIdAsync(long accountId)
        {
            return await _historyRepo.Get().Where(x => x.AccountId == accountId).Select(x => new VehicleUsageHistoryDto
            {
                Id = x.Id,
                VehicleId = x.VehicleId,
                AccountId = x.AccountId,
                StartTime = x.StartTime,
                EndTime = x.EndTime
            }).ToListAsync();
        }

        public async Task CreateAsync(long accountId, VehicleUsageHistoryCreateDto dto)
        {
            var vehicle = await _vehicleRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == dto.VehicleId);

            if (vehicle == null)
                throw new Exception($"Xe với Id {dto.VehicleId} không tồn tại");

            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == accountId);

            if (account == null)
                throw new Exception($"Người dùng với Id {accountId} không tồn tại");

            var entity = new VehicleUsageHistory
            {
                VehicleId = dto.VehicleId,
                AccountId = accountId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            await _historyRepo.AddAsync(entity);
            await _historyRepo.SaveChangesAsync();
        }


        public async Task UpdateAsync(long accountId, VehicleUsageHistoryUpdateDto dto)
        {
            var entity = await _historyRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Không tìm thấy lịch sử sử dụng xe");

            // Không cho user sửa lịch sử của người khác
            if (entity.AccountId != accountId)
                throw new Exception("Bạn không có quyền chỉnh sửa lịch sử sử dụng xe của người khác");

            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

            _historyRepo.Update(entity);
            await _historyRepo.SaveChangesAsync();
        }
    }
}
