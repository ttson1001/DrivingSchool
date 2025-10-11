using BEAPI.Dtos.account;
using BEAPI.Entities;

namespace BEAPI.Services.IService
{
    public interface IAccountService
    {
        Task<Account> RegisterAsync(CreateAccountRequest request);
        Task<string> LoginAsync(LoginDto dto);
    }
}
