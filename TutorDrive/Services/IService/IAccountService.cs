using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Account;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Staff.TutorDrive.Dtos.Accounts;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IAccountService
    {
        Task<Account> RegisterAsync(CreateAccountRequest request);
        Task ForgotPasswordAsync(ForgotPasswordRequest dto);
        Task ResetPasswordAsync(ResetPasswordRequest dto);
        Task SetStatusAsync(long accountId, bool isActive);
        Task<LoginReponseDto> LoginAsync(LoginDto dto);
        Task<PagedResult<MeDto>> SearchAccountsAsync(string? keyword, long? roleId, int page, int pageSize);
        Task<AccountDto> GetAccountByIdAsync(long id);
        Task<AccountDto> UpdateAccountAsync(long id, AccountUpdateDto dto);
        Task CreateAccountAsync(AccountCreateDto dto);
        Task<MeDto> GetMeAsync(long accountId);
        Task<StudentProfile> UpdateAsync(long id, UpdateStudentProfileDto dto);
        Task ChangePasswordAsync(long accountId, ChangePasswordRequest dto);
        Task<InstructorProfile> UpdateInstructorProfileAsync(long accountId, UpdateInstructorProfileDto dto);
        Task<List<InstructorDto>> GetAllInstructorsAsync();
    }
}
