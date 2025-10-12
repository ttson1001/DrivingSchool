namespace BEAPI.Services
{
    using BEAPI.Exceptions;
    using BEAPI.Services.IServices;
    using global::BEAPI.Dtos.account;
    using global::BEAPI.Entities;
    using global::BEAPI.Repositories;
    using global::BEAPI.Services.IService;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
    using System.Text;
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IJwtService _jwtService;
        private readonly IRepository<StudentProfile> _studentRepo;

        public AccountService(
            IRepository<Account> accountRepo,
            IJwtService jwtService,
            IRepository<Role> roleRepo,
            IRepository<StudentProfile> studentRepo)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
            _roleRepo = roleRepo;
            _jwtService = jwtService;
        }

        public async Task<Account> RegisterAsync(CreateAccountRequest request)
        {
            var accountExist = await _accountRepo.Get().Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (accountExist != null)
                throw new Exception("Email đã tồn tại");

            var hashed = HashPassword(request.Password);
            var role = await _roleRepo.Get().FirstAsync(x => x.Name == "Student");
            var account = new Account
            {
                Email = request.Email,
                PasswordHash = hashed,
                FullName = request.FullName,
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow,
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();
            return account;
        }

        public async Task<LoginReponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _accountRepo.Get().Include(x => x.Role).FirstOrDefaultAsync(u => u.Email == dto.Email) ?? throw new KeyNotFoundException("User not found");

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception(ExceptionConstant.InvalidCredentials);

            var token = _jwtService.GenerateToken(user, null);
            await _accountRepo.SaveChangesAsync();
            return new LoginReponseDto
            {
                Token = token
            };
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

    }

}
