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
    using TutorDrive.Entities.Enum;
    using TutorDrive.Exceptions;

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

        public async Task SetStatusAsync(long accountId, bool isActive)
        {
            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == accountId)
                ?? throw new Exception("Không tìm thấy tài khoản.");

            if (account.RoleId == 1)
                throw new Exception("Không thể khóa tài khoản Admin.");

            account.Status = isActive ? AccountStatus.Active : AccountStatus.Inactive;

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();

            string html = EmailTemplateHelper.BuildAccountStatusEmail(account.FullName, isActive);

            await _emailService.SendEmailAsync(
                account.Email,
                isActive ? "Tài khoản đã được mở khóa" : "Tài khoản đã bị khóa",
                html
            );
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
            var user = await _accountRepo.Get()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email)
                ?? throw new KeyNotFoundException("Không tìm thấy tài khoản");

            if (user.Status == AccountStatus.Inactive)
                throw new Exception("Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception(ExceptionConstant.InvalidCredentials);

            var token = _jwtService.GenerateToken(user, null);

            return new LoginReponseDto
            {
                Token = token
            };
        }

        public async Task<List<InstructorDto>> GetAllInstructorsAsync()
        {
            var instructors = await _staffRepo.Get()
                .Include(i => i.Account)
                .OrderBy(i => i.Account.FullName)
                .Where(i => i.Account.Status == AccountStatus.Active)
                .ToListAsync();

            return instructors.Select(i => new InstructorDto
            {
                Id = i.Id,
                FullName = i.Account.FullName,
                Avatar = i.Account.Avatar,
                ExperienceYears = i.ExperienceYears,
                Description = i.Description
            }).ToList();
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<PagedResult<MeDto>> SearchAccountsAsync(string? keyword, long? roleId, int page, int pageSize)
        {
            IQueryable<Account> query = _accountRepo.Get().Include(x => x.Role);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(x =>
                    (x.Email ?? "").ToLower().Contains(keyword) ||
                    (x.FullName ?? "").ToLower().Contains(keyword) ||
                    (x.PhoneNumber ?? "").ToLower().Contains(keyword)
                );
            }

            if (roleId.HasValue && roleId > 0)
                query = query.Where(x => x.RoleId == roleId);

            var totalItems = await query.CountAsync();

            var accounts = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var result = new List<MeDto>();

            foreach (var account in accounts)
            {
                var me = new MeDto
                {
                    AccountId = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Role = account.Role.Name,
                    PhoneNumber = account.PhoneNumber,
                    Avatar = account.Avatar,
                    Status = account.Status,
                };

                if (account.Role.Name == "Instructor")
                {
                    var staff = await _staffRepo.Get().FirstOrDefaultAsync(s => s.AccountId == account.Id);

                    if (staff != null)
                    {
                        me.LicenseNumber = staff.LicenseNumber;
                        me.ExperienceYears = staff.ExperienceYears;
                        me.Description = staff.Description;
                    }
                }
                else if (account.Role.Name == "Student")
                {
                    var student = await _studentRepo.Get()
                        .Include(s => s.Address).ThenInclude(a => a.Ward)
                        .Include(s => s.Address).ThenInclude(a => a.Province)
                        .FirstOrDefaultAsync(s => s.AccountId == account.Id);

                    if (student != null)
                    {
                        me.CMND = student.CMND;
                        me.DOB = student.DOB;

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

                result.Add(me);
            }

            return new PagedResult<MeDto>
            {
                Items = result,
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
            var role = await _roleRepo.Get()
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId)
                ?? throw new Exception("Role không tồn tại.");

            if (await _accountRepo.Get().AnyAsync(a => a.Email == dto.Email))
                throw new Exception("Email đã tồn tại.");

            var rawPassword = GenerateRandomPassword();
            var hashedPassword = HashPassword(rawPassword);

            var account = new Account
            {
                Email = dto.Email,
                FullName = dto.FullName,
                Avatar = dto.Avartar,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();

            if (role.Name.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
            {
                var staff = new InstructorProfile
                {
                    AccountId = account.Id,
                    LicenseNumber = dto.LicenseNumber ?? "N/A",
                    ExperienceYears = dto.ExperienceYears ?? 0,
                    Description = ""
                };

                await _staffRepo.AddAsync(staff);
                await _staffRepo.SaveChangesAsync();
            }

            await _emailService.SendEmailAsync(
                account.Email,
                "Chào mừng bạn đến vs TutorDrive",
                $@"
            <h3>Xin chào {account.FullName},</h3>
            <p>Tài khoản của bạn đã được tạo thành công.</p>
            <p><b>Email:</b> {account.Email}</p>
            <p><b>Mật khẩu:</b> {rawPassword}</p>
            <p>Vui lòng đăng nhập và đổi mật khẩu ngay.</p>
        "
            );
        }


        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var rnd = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public async Task<AccountDto> UpdateAccountAsync(long id, AccountUpdateDto dto)
        {
            var account = await _accountRepo.Get()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                account.FullName = dto.FullName;

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
                Avatar = account.Avatar,
                Status = account.Status,
            };

            if (roleName == "Instructor")
            {
                var staff = await _staffRepo.Get().FirstOrDefaultAsync(s => s.AccountId == account.Id);
                if (staff != null)
                {
                    me.LicenseNumber = staff.LicenseNumber;
                    me.ExperienceYears = staff.ExperienceYears;
                    me.Description = staff.Description;
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

        public async Task ChangePasswordAsync(long accountId, ChangePasswordRequest dto)
        {
            var account = await _accountRepo.Get()
                .FirstOrDefaultAsync(x => x.Id == accountId)
                ?? throw new Exception("Không tìm thấy tài khoản.");

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, account.PasswordHash))
                throw new Exception("Mật khẩu cũ không chính xác.");

            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();
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

        public async Task<InstructorProfile> UpdateInstructorProfileAsync(long accountId, UpdateInstructorProfileDto dto)
        {
            var profile = await _staffRepo.Get()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            bool isNewProfile = false;

            if (profile == null)
            {
                var account = await _accountRepo.Get()
                    .FirstOrDefaultAsync(x => x.Id == accountId);

                if (account == null)
                    throw new Exception("Không tìm thấy tài khoản tương ứng");

                profile = new InstructorProfile
                {
                    Account = account
                };

                isNewProfile = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.LicenseNumber))
                profile.LicenseNumber = dto.LicenseNumber;

            if (dto.ExperienceYears.HasValue)
                profile.ExperienceYears = dto.ExperienceYears.Value;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                profile.Description = dto.Description;

            if (profile.Account != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    profile.Account.FullName = dto.FullName;

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    profile.Account.PhoneNumber = dto.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(dto.Avatar))
                    profile.Account.Avatar = dto.Avatar;
            }

            if (!isNewProfile)
                _staffRepo.Update(profile);
            else
                await _staffRepo.AddAsync(profile);

            await _staffRepo.SaveChangesAsync();

            return profile;
        }
    }
}
