using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Vehicle;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class VehicleUsageHistoryService : IVehicleUsageHistoryService
    {
        private readonly IRepository<VehicleUsageHistory> _historyRepo;
        private readonly IRepository<Vehicle> _vehicleRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<InstructorProfile> _instructorProfileRepo;

        public VehicleUsageHistoryService(
            IRepository<VehicleUsageHistory> historyRepo,
            IRepository<Vehicle> vehicleRepo,
            IRepository<Account> accountRepo,
            IRepository<InstructorProfile> instructorProfileRepo)
        {
            _historyRepo = historyRepo;
            _vehicleRepo = vehicleRepo;
            _accountRepo = accountRepo;
            _instructorProfileRepo = instructorProfileRepo;
        }

        public async Task<List<VehicleUsageHistoryDto>> GetAllAsync()
        {
            return await _historyRepo.Get()
                .Include(x => x.Account).ThenInclude(a => a.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(x => x.Account).ThenInclude(a => a.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(x => x.Account).ThenInclude(a => a.InstructorProfile)
                .Include(x => x.Vehicle)
                .OrderByDescending(x => x.StartTime)
                .Select(x => new VehicleUsageHistoryDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,

                    Account = new MeDto
                    {
                        AccountId = x.Account.Id,
                        Email = x.Account.Email,
                        FullName = x.Account.FullName,
                        PhoneNumber = x.Account.PhoneNumber,
                        Avatar = x.Account.Avatar,
                        LicenseNumber = x.Account.InstructorProfile == null ? null : x.Account.InstructorProfile.LicenseNumber,
                        ExperienceYears = x.Account.InstructorProfile == null ? null : x.Account.InstructorProfile.ExperienceYears,

                    },

                    Vehicle = new VehicleDto
                    {
                        Id = x.Vehicle.Id,
                        PlateNumber = x.Vehicle.PlateNumber,
                        Brand = x.Vehicle.Brand,
                        ImageUrl = x.Vehicle.ImageUrl,
                        Model = x.Vehicle.Model,
                        Status = x.Vehicle.Status
                    }
                })
                .ToListAsync();
        }

        public async Task<VehicleUsageHistoryDto> GetByIdAsync(long id)
        {
            var x = await _historyRepo.Get()
                .Include(v => v.Account).ThenInclude(a => a.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(v => v.Account).ThenInclude(a => a.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(v => v.Account).ThenInclude(a => a.InstructorProfile)
                .Include(v => v.Vehicle)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (x == null)
                throw new Exception("Không tìm thấy");

            return new VehicleUsageHistoryDto
            {
                Id = x.Id,
                StartTime = x.StartTime,
                EndTime = x.EndTime,

                Account = new MeDto
                {
                    AccountId = x.Account.Id,
                    Email = x.Account.Email,
                    FullName = x.Account.FullName,
                    PhoneNumber = x.Account.PhoneNumber,
                    Avatar = x.Account.Avatar,
                    LicenseNumber = x.Account.InstructorProfile?.LicenseNumber,
                    ExperienceYears = x.Account.InstructorProfile?.ExperienceYears,
                },

                Vehicle = new VehicleDto
                {
                    Id = x.Vehicle.Id,
                    PlateNumber = x.Vehicle.PlateNumber,
                    Brand = x.Vehicle.Brand,
                    ImageUrl = x.Vehicle.ImageUrl,
                    Model = x.Vehicle.Model,
                    Status = x.Vehicle.Status
                }
            };
        }
        public async Task<List<VehicleUsageHistoryDto>> GetAllByVehicleIdAsync(long vehicleId)
        {
            var list = await _historyRepo.Get()
                .Where(x => x.VehicleId == vehicleId)
                .Include(x => x.Account).ThenInclude(a => a.StudentProfile)
                .Include(x => x.Account).ThenInclude(a => a.InstructorProfile)
                .Include(x => x.Vehicle)
                .ToListAsync();

            return list.Select(x => new VehicleUsageHistoryDto
            {
                Id = x.Id,
                StartTime = x.StartTime,
                EndTime = x.EndTime,

                Account = new MeDto
                {
                    AccountId = x.Account.Id,
                    Email = x.Account.Email,
                    FullName = x.Account.FullName,
                    PhoneNumber = x.Account.PhoneNumber,
                    Avatar = x.Account.Avatar,
                    LicenseNumber = x.Account.InstructorProfile?.LicenseNumber,
                    ExperienceYears = x.Account.InstructorProfile?.ExperienceYears,
                    CMND = x.Account.StudentProfile?.CMND,
                    DOB = x.Account.StudentProfile?.DOB,
                },

                Vehicle = new VehicleDto
                {
                    Id = x.Vehicle.Id,
                    PlateNumber = x.Vehicle.PlateNumber,
                    ImageUrl = x.Vehicle.ImageUrl,
                    Brand = x.Vehicle.Brand,
                    Model = x.Vehicle.Model,
                    Status = x.Vehicle.Status
                }
            }).ToList();
        }

        public async Task<List<VehicleUsageHistoryDto>> GetAllByAccountIdAsync(long accountId)
        {
            var list = await _historyRepo.Get()
                .Where(x => x.AccountId == accountId)
                .Include(x => x.Account).ThenInclude(a => a.StudentProfile)
                .Include(x => x.Account).ThenInclude(a => a.InstructorProfile)
                .Include(x => x.Vehicle)
                .ToListAsync();

            return list.Select(x => new VehicleUsageHistoryDto
            {
                Id = x.Id,
                StartTime = x.StartTime,
                EndTime = x.EndTime,

                Account = new MeDto
                {
                    AccountId = x.Account.Id,
                    Email = x.Account.Email,
                    FullName = x.Account.FullName,
                    PhoneNumber = x.Account.PhoneNumber,
                    Avatar = x.Account.Avatar,
                    LicenseNumber = x.Account.InstructorProfile?.LicenseNumber,
                    ExperienceYears = x.Account.InstructorProfile?.ExperienceYears,
                    CMND = x.Account.StudentProfile?.CMND,
                    DOB = x.Account.StudentProfile?.DOB,
                },

                Vehicle = new VehicleDto
                {
                    Id = x.Vehicle.Id,
                    PlateNumber = x.Vehicle.PlateNumber,
                    ImageUrl = x.Vehicle.ImageUrl,
                    Brand = x.Vehicle.Brand,
                    Model = x.Vehicle.Model,
                    Status = x.Vehicle.Status
                }
            }).ToList();
        }

        public async Task CreateAsync(long accountId, VehicleUsageHistoryCreateDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                throw new Exception("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");

            var vehicle = await _vehicleRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == dto.VehicleId);
            if (vehicle == null)
                throw new Exception($"Xe với Id {dto.VehicleId} không tồn tại");

            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
                throw new Exception($"Người dùng với Id {accountId} không tồn tại");

            var instructor = await _instructorProfileRepo.Get()
                .FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (instructor == null)
                throw new Exception("Người dùng này không phải là giảng viên.");

            var now = DateTime.UtcNow;

            var hiệnĐangMượnXe = await _historyRepo.Get()
                .AnyAsync(h =>
                    h.AccountId == accountId &&
                    h.EndTime > now
                );

            if (hiệnĐangMượnXe)
                throw new Exception("Giáo viên hiện đang sử dụng một xe khác. Không thể mượn thêm.");

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

            if (entity.AccountId != accountId)
                throw new Exception("Bạn không có quyền chỉnh sửa");

            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Status = dto.Status;

            _historyRepo.Update(entity);
            await _historyRepo.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(VehicleUsageHistoryStatusUpdateDto dto)
        {
            var entity = await _historyRepo.Get()
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Không tìm thấy lịch sử sử dụng xe");

            if (entity.Status == VehicleUsageStatus.Approved)
                throw new Exception("Lịch sử đã được phê duyệt, không thể thay đổi trạng thái.");

            if (entity.Status == VehicleUsageStatus.Cancelled)
                throw new Exception("Lịch sử đã bị từ chối, không thể thay đổi trạng thái.");

            if (entity.Status == VehicleUsageStatus.Pending)
            {
                if (dto.Status == VehicleUsageStatus.Approved)
                {
                    entity.Vehicle.Status = "Inactive";
                    _vehicleRepo.Update(entity.Vehicle);
                }

                entity.Status = dto.Status;
                _historyRepo.Update(entity);
                await _historyRepo.SaveChangesAsync();
                return;
            }
        }

    }
}
