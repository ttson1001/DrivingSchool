using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.DriverLicense;
using TutorDrive.Entities;
using TutorDrive.Exceptions;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class DriverLicenseService : IDriverLicenseService
    {
        private readonly IRepository<DriverLicense> _licenseRepo;

        public DriverLicenseService(IRepository<DriverLicense> licenseRepo)
        {
            _licenseRepo = licenseRepo;
        }

        public async Task<DriverLicenseDto> CreateAsync(CreateDriverLicenseDto dto)
        {
            var exist = await _licenseRepo.Get().AnyAsync(x => x.Name == dto.Name);
            if (exist)
                throw new Exception("Loại bằng lái đã tồn tại.");

            var entity = new DriverLicense
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _licenseRepo.AddAsync(entity);
            await _licenseRepo.SaveChangesAsync();

            return new DriverLicenseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        public async Task<DriverLicenseDto> UpdateAsync(UpdateDriverLicenseDto dto)
        {
            var license = await _licenseRepo.Get().FirstOrDefaultAsync(x => x.Id == dto.Id)
                ?? throw new KeyNotFoundException("Không tìm thấy bằng lái.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                license.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                license.Description = dto.Description;

            _licenseRepo.Update(license);
            await _licenseRepo.SaveChangesAsync();

            return new DriverLicenseDto
            {
                Id = license.Id,
                Name = license.Name,
                Description = license.Description
            };
        }

        public async Task<DriverLicenseDto> GetByIdAsync(long id)
        {
            var license = await _licenseRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Không tìm thấy bằng lái.");

            return new DriverLicenseDto
            {
                Id = license.Id,
                Name = license.Name,
                Description = license.Description
            };
        }

        public async Task<PagedResult<DriverLicenseDto>> SearchAsync(string? keyword, int page, int pageSize)
        {
            var query = _licenseRepo.Get();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DriverLicenseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync();

            return new PagedResult<DriverLicenseDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
