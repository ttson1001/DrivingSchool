using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Registration;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IRegistrationFullService
    {
        Task RegisterFullAsync(long accountId, RegistrationFullCreateDto dto);
        Task<PagedResult<RegistrationListItemDto>> SearchAsync(RegistrationSearchDto filter);
        Task<PagedResult<RegistrationListItemDto>> GetByAccountIdAsync(long accountId, RegistrationSearchDto filter);
        Task UpdateStatusAsync(UpdateRegistrationStatusDto dto);

    }

}
