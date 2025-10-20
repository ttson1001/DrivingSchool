using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.DriverLicense;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IDriverLicenseService
    {
        Task<DriverLicenseDto> CreateAsync(CreateDriverLicenseDto dto);
        Task<DriverLicenseDto> UpdateAsync(UpdateDriverLicenseDto dto);
        Task<DriverLicenseDto> GetByIdAsync(long id);
        Task<PagedResult<DriverLicenseDto>> SearchAsync(string? keyword, int page, int pageSize);
    }
}
