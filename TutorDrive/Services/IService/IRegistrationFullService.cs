using TutorDrive.Dtos.Registration;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IRegistrationFullService
    {
        Task RegisterFullAsync(long accountId, RegistrationFullCreateDto dto);
    }

}
