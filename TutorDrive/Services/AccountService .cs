namespace TutorDrive.Services
{
    using global::TutorDrive.Dtos.account;
    using global::TutorDrive.Entities;
    using global::TutorDrive.Repositories;
    using global::TutorDrive.Services.IService;
    using Microsoft.EntityFrameworkCore;
    using TutorDrive.Dtos.Account;
    using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
    using TutorDrive.Dtos.Common;
    using TutorDrive.Dtos.Staff.TutorDrive.Dtos.Accounts;
    using TutorDrive.Exceptions;
    using TutorDrive.Services.IService.TutorDrive.Services.IService;
    using TutorDrive.Services.IServices;

    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IJwtService _jwtService;
        private readonly IRepository<StudentProfile> _studentRepo;
        private readonly IRepository<InstructorProfile> _staffRepo;
        private readonly IRepository<Address> _addressRepo;
        private readonly IRepository<Ward> _wardRepo;
        private readonly IRepository<Province> _provinceRepo;
        private readonly IEmailService _emailService;

        public AccountService(
            IRepository<Account> accountRepo,
            IJwtService jwtService,
            IRepository<Role> roleRepo,
            IRepository<StudentProfile> studentRepo,
            IRepository<Address> addressRepo,
            IRepository<InstructorProfile> staffRepo,
            IRepository<Ward> wardRepo,
            IRepository<Province> provinceRepo,
            IEmailService emailService)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
            _roleRepo = roleRepo;
            _jwtService = jwtService;
            _staffRepo = staffRepo;
            _addressRepo = addressRepo;
            _wardRepo = wardRepo;
            _provinceRepo = provinceRepo;
            _emailService = emailService;
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

        public async Task ResetPasswordAsync(ResetPasswordRequest dto)
        {
            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (account == null)
                throw new Exception("Email không tồn tại");

            if (account.ResetOtp == null || account.ResetOtpExpiry == null)
                throw new Exception("OTP chưa được tạo");

            if (DateTime.UtcNow > account.ResetOtpExpiry)
                throw new Exception("OTP đã hết hạn");

            if (!account.ResetOtp.Equals(dto.Otp))
                throw new Exception("OTP không đúng");

            account.PasswordHash = HashPassword(dto.NewPassword);
            account.ResetOtp = null;
            account.ResetOtpExpiry = null;

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest dto)
        {
            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (account == null)
                throw new Exception("Email không tồn tại trong hệ thống");

            string otp = GenerateOtp();

            account.ResetOtp = otp;
            account.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(5);

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();

            await _emailService.SendOtpEmailAsync(account.Email, otp);
        }

        private string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
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
                    Avatar = x.Avatar,
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
                Avatar = account.Avatar,
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

            if (role.Name.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
            {
                var staff = new InstructorProfile
                {
                    Account = account,
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
                Avatar = account.Avatar,
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

            var roleName = account.Role.Name;

            var me = new MeDto
            {
                AccountId = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role.Name,
                PhoneNumber = account.PhoneNumber,
                Avatar = account.Avatar
            };

            if (roleName == "Instructor")
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
                            ProvinceName = student.Address.Province?.Name,
                            WardId = student.Address.WardId,
                            ProvinceId = student.Address.ProvinceId
                        };
                    }
                }
            }

            return me;
        }

        public async Task<StudentProfile> UpdateAsync(long accountId, UpdateStudentProfileDto dto)
        {
            var profile = await _studentRepo.Get()
                .Include(x => x.Address)
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            bool isNewProfile = false;

            if (profile == null)
            {
                var account = await _accountRepo.Get().FirstOrDefaultAsync(x => x.Id == accountId);
                if (account == null)
                    throw new Exception("Không tìm thấy tài khoản tương ứng");

                profile = new StudentProfile
                {
                    Account = account,
                    Status = "Active"
                };

                isNewProfile = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.CMND))
                profile.CMND = dto.CMND;

            if (dto.DOB.HasValue)
                profile.DOB = dto.DOB;

            if (profile.Account != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    profile.Account.FullName = dto.FullName;

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    profile.Account.PhoneNumber = dto.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(dto.Avatar))
                    profile.Account.Avatar = dto.Avatar;
            }

            if (dto.Address != null)
            {
                var ward = await _wardRepo.Get().FirstOrDefaultAsync(x => x.Code == dto.Address.WardCode);
                var province = await _provinceRepo.Get().FirstOrDefaultAsync(x => x.Code == dto.Address.ProvinceCode);

                if (dto.Address.Id > 0)
                {
                    var address = await _addressRepo.Get().FirstOrDefaultAsync(x => x.Id == dto.Address.Id);
                    if (address == null)
                        throw new Exception("Không tìm thấy địa chỉ");

                    if (!string.IsNullOrWhiteSpace(dto.Address.FullAddress))
                        address.FullAddress = dto.Address.FullAddress;

                    if (!string.IsNullOrWhiteSpace(dto.Address.Street))
                        address.Street = dto.Address.Street;

                    address.Ward = ward;
                    address.Province = province;

                    _addressRepo.Update(address);
                    profile.Address = address;
                }
                else
                {
                    var newAddress = new Address
                    {
                        FullAddress = dto.Address.FullAddress ?? "",
                        Street = dto.Address.Street ?? "",
                        Ward = ward,
                        Province = province,
                    };

                    await _addressRepo.AddAsync(newAddress);
                    profile.Address = newAddress;
                }
            }

            if (!isNewProfile)
                _studentRepo.Update(profile);
            else
            {
                await _studentRepo.AddAsync(profile);
            }
            await _studentRepo.SaveChangesAsync();

            return profile;
        }

    }
}
