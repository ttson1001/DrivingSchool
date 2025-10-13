using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Vehicle;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace TutorDrive.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<Vehicle> _vehicleRepo;

        public VehicleService(IRepository<Vehicle> vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        public async Task<List<VehicleDto>> GetAllVehiclesAsync()
        {
            return await _vehicleRepo.Get()
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    PlateNumber = v.PlateNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Status = v.Status
                })
                .ToListAsync();
        }

        public async Task CreateVehicleAsync(VehicleCreateDto dto)
        {
            var vehicle = new Vehicle
            {
                PlateNumber = dto.PlateNumber,
                Brand = dto.Brand,
                Model = dto.Model,
                Status = dto.Status
            };

            await _vehicleRepo.AddAsync(vehicle);
            await _vehicleRepo.SaveChangesAsync();
        }

        public async Task UpdateVehicleAsync( VehicleUpdateDto dto)
        {
            var vehicle = await _vehicleRepo.Get().FirstOrDefaultAsync( x => x.Id == dto.Id);
            if (vehicle == null) throw new Exception("Xe không tồn tại");

            vehicle.Brand = dto.Brand;
            vehicle.Model = dto.Model;
            vehicle.Status = dto.Status;

            _vehicleRepo.Update(vehicle);
            await _vehicleRepo.SaveChangesAsync();
        }

        public async Task<PagedResult<VehicleDto>> SearchVehiclesAsync(string? keyword, int page, int pageSize)
        {
            var query = _vehicleRepo.Get();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(v =>
                    v.PlateNumber.Contains(keyword) ||
                    v.Brand.Contains(keyword) ||
                    v.Model.Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(v => v.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    PlateNumber = v.PlateNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Status = v.Status
                })
                .ToListAsync();

            return new PagedResult<VehicleDto>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }
    }
}
