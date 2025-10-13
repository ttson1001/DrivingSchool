using TutorDrive.Dtos.account;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IAccountService
    {
        Task<Account> RegisterAsync(CreateAccountRequest request);
        Task<LoginReponseDto> LoginAsync(LoginDto dto);
    }
}
