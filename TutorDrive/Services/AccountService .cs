namespace TutorDrive.Services
{
    using global::TutorDrive.Dtos.account;
    using global::TutorDrive.Entities;
    using global::TutorDrive.Repositories;
    using global::TutorDrive.Services.IService;
    using Microsoft.EntityFrameworkCore;
    using TutorDrive.Database;
    using TutorDrive.Dtos.Account;
    using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
    using TutorDrive.Dtos.Common;
    using TutorDrive.Dtos.Staff.TutorDrive.Dtos.Accounts;
    using TutorDrive.Exceptions;
    using TutorDrive.Services.IServices;

    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IJwtService _jwtService;
        private readonly IRepository<StudentProfile> _studentRepo;
        private readonly IRepository<Staff> _staffRepo;

        public AccountService(
            IRepository<Account> accountRepo,
            IJwtService jwtService,
            IRepository<Role> roleRepo,
            IRepository<StudentProfile> studentRepo,
            IRepository<Staff> staffRepo)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
            _roleRepo = roleRepo;
            _jwtService = jwtService;
            _staffRepo = staffRepo;
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

        public async Task<PagedResult<AccountDto>> SearchAccountsAsync(string? keyword, int page, int pageSize)
        {
            IQueryable<Account> query = _accountRepo.Get().Include(x => x.Role);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Email.Contains(keyword) || x.FullName.Contains(keyword));

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                    RoleName = x.Role.Name,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<AccountDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<AccountDto> GetAccountByIdAsync(long id)
        {
            var account = await _accountRepo.Get()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            return new AccountDto
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                RoleName = account.Role.Name,
                CreatedAt = account.CreatedAt
            };
        }


        public async Task CreateAccountAsync(AccountCreateDto dto)
        {
            var role = await _roleRepo.Get().Where(r => r.Id == dto.RoleId).FirstAsync();
            if (role == null)
                throw new Exception("Role không tồn tại.");

            if (await _accountRepo.Get().FirstOrDefaultAsync(a => a.Email == dto.Email) != null)
                throw new Exception("Email đã tồn tại.");

            var account = new Account
            {
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.RoleId
            };

            await _accountRepo.AddAsync(account);

            if (role.Name.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                var staff = new Staff
                {
                    AccountId = account.Id,
                    LicenseNumber = dto.LicenseNumber ?? "N/A",
                    ExperienceYears = dto.ExperienceYears ?? 0
                };
                await _staffRepo.AddAsync(staff);
                await _staffRepo.SaveChangesAsync();
            }
        }
        public async Task<AccountDto> UpdateAccountAsync(long id, AccountUpdateDto dto)
        {
            var account = await _accountRepo.Get()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                account.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                account.PasswordHash = HashPassword(dto.Password);

            if (dto.RoleId.HasValue)
                account.RoleId = dto.RoleId.Value;

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();

            return new AccountDto
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                RoleName = account.Role.Name,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<MeDto> GetMeAsync(long accountId)
        {
            var account = await _accountRepo.Get()
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
                throw new Exception("Account not found");

            var roleName = account.Role.Name.ToLower();

            var me = new MeDto
            {
                AccountId = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role.Name
            };

            if (roleName == "Teacher")
            {
                var staff = await _staffRepo.Get().FirstOrDefaultAsync(s => s.AccountId == account.Id);
                if (staff != null)
                {
                    me.LicenseNumber = staff.LicenseNumber;
                    me.ExperienceYears = staff.ExperienceYears;
                }
            }
            else if (roleName == "Student")
            {
                var student = await _studentRepo.Get()
                    .Include(s => s.Address)
                        .ThenInclude(a => a.Ward)
                    .Include(s => s.Address)
                        .ThenInclude(a => a.Province)
                    .FirstOrDefaultAsync(s => s.AccountId == account.Id);

                if (student != null)
                {
                    me.CMND = student.CMND;
                    me.DOB = student.DOB;
                    me.Status = student.Status;

                    if (student.Address != null)
                    {
                        me.Address = new AddressDto
                        {
                            FullAddress = student.Address.FullAddress,
                            Street = student.Address.Street,
                            WardName = student.Address.Ward?.Name,
                            ProvinceName = student.Address.Province?.Name
                        };
                    }
                }
            }

            return me;
        }
    }
}
