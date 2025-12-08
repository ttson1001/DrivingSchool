using Moq;
using Xunit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services;
using TutorDrive.Services.IService;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Account;
using MockQueryable.EntityFrameworkCore;

namespace TutorDriveTest.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IRepository<Account>> _accountRepo;
        private readonly Mock<IRepository<Role>> _roleRepo;
        private readonly Mock<IRepository<StudentProfile>> _studentRepo;
        private readonly Mock<IRepository<InstructorProfile>> _staffRepo;
        private readonly Mock<IRepository<Address>> _addressRepo;
        private readonly Mock<IRepository<Ward>> _wardRepo;
        private readonly Mock<IRepository<Province>> _provinceRepo;

        private readonly Mock<IJwtService> _jwtService;
        private readonly Mock<IEmailService> _emailService;

        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _accountRepo = new Mock<IRepository<Account>>();
            _roleRepo = new Mock<IRepository<Role>>();
            _studentRepo = new Mock<IRepository<StudentProfile>>();
            _staffRepo = new Mock<IRepository<InstructorProfile>>();
            _addressRepo = new Mock<IRepository<Address>>();
            _wardRepo = new Mock<IRepository<Ward>>();
            _provinceRepo = new Mock<IRepository<Province>>();

            _jwtService = new Mock<IJwtService>();
            _emailService = new Mock<IEmailService>();

            _service = new AccountService(
                _accountRepo.Object,
                _jwtService.Object,
                _roleRepo.Object,
                _studentRepo.Object,
                _addressRepo.Object,
                _staffRepo.Object,
                _wardRepo.Object,
                _provinceRepo.Object,
                _emailService.Object
            );
        }

        //------------------------ REGISTER --------------------------
        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
        {
            var data = new List<Account>
            {
                new Account { Email = "a@gmail.com" }
            }.AsQueryable();

            _accountRepo.Setup(x => x.Get()).Returns(data.BuildMock());

            var req = new CreateAccountRequest
            {
                Email = "a@gmail.com",
                Password = "123",
                FullName = "Test"
            };

            await Assert.ThrowsAsync<Exception>(() => _service.RegisterAsync(req));
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateAccount()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());
            _roleRepo.Setup(x => x.Get()).Returns(
                new List<Role> { new Role { Id = 1, Name = "Student" } }.AsQueryable().BuildMock()
            );

            Account captured = null;

            _accountRepo
                .Setup(x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, ct) => captured = a)
                .Returns(Task.CompletedTask);

            _accountRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var req = new CreateAccountRequest
            {
                Email = "new@gmail.com",
                Password = "123",
                FullName = "User"
            };

            await _service.RegisterAsync(req);

            Assert.NotNull(captured);
            Assert.Equal("new@gmail.com", captured.Email);
            Assert.Equal("User", captured.FullName);
            Assert.Equal(1, captured.RoleId);
        }

        //------------------------ LOGIN --------------------------
        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenEmailNotFound()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());

            var dto = new LoginDto { Email = "none@gmail.com", Password = "123" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
        {
            var acc = new Account
            {
                Email = "a@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                Status = AccountStatus.Active,
                Role = new Role { Name = "Student" }
            };

            _accountRepo.Setup(x => x.Get()).Returns(new List<Account> { acc }.AsQueryable().BuildMock());

            var dto = new LoginDto { Email = "a@gmail.com", Password = "wrong" };

            await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken()
        {
            var acc = new Account
            {
                Email = "a@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
                Status = AccountStatus.Active,
                Role = new Role { Name = "Student" }
            };

            _accountRepo.Setup(x => x.Get()).Returns(new List<Account> { acc }.AsQueryable().BuildMock());

            _jwtService.Setup(x => x.GenerateToken(acc, null)).Returns("fake_token");

            var result = await _service.LoginAsync(new LoginDto
            {
                Email = "a@gmail.com",
                Password = "123"
            });

            Assert.Equal("fake_token", result.Token);
        }

        //------------------------ SET STATUS --------------------------
        [Fact]
        public async Task SetStatusAsync_ShouldThrow_WhenAccountNotFound()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());
            await Assert.ThrowsAsync<Exception>(() => _service.SetStatusAsync(1, true));
        }

        [Fact]
        public async Task SetStatusAsync_ShouldSendEmail()
        {
            var acc = new Account { Id = 5, Email = "a@gmail.com", RoleId = 2 };

            _accountRepo.Setup(x => x.Get()).Returns(new List<Account> { acc }.AsQueryable().BuildMock());
            _accountRepo.Setup(x => x.Update(It.IsAny<Account>()));
            _accountRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.SetStatusAsync(5, false);

            _emailService.Verify(
                x => x.SendEmailAsync(
                    "a@gmail.com",
                    "Tài khoản đã bị khóa",
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        //------------------------ FORGOT PASSWORD --------------------------
        [Fact]
        public async Task ForgotPasswordAsync_ShouldThrow_WhenEmailNotFound()
        {
            var emptyList = new List<Account>().AsQueryable();
            _accountRepo.Setup(x => x.Get()).Returns(emptyList.BuildMock());

            await Assert.ThrowsAsync<Exception>(() =>
                _service.ForgotPasswordAsync(new ForgotPasswordRequest
                {
                    Email = "none@gmail.com"
                }));
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldSendOtpEmail()
        {
            var acc = new Account { Email = "a@gmail.com" };
            var data = new List<Account> { acc }.AsQueryable();

            _accountRepo.Setup(x => x.Get()).Returns(data.BuildMock());

            _accountRepo.Setup(x => x.Update(It.IsAny<Account>()));
            _accountRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.ForgotPasswordAsync(
                new ForgotPasswordRequest { Email = "a@gmail.com" });

            _emailService.Verify(
                x => x.SendOtpEmailAsync("a@gmail.com", It.IsAny<string>()),
                Times.Once);
        }

        //------------------------ RESET PASSWORD --------------------------
        [Fact]
        public async Task ResetPasswordAsync_ShouldThrow_WhenEmailNotExist()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());

            await Assert.ThrowsAsync<Exception>(() =>
                _service.ResetPasswordAsync(new ResetPasswordRequest
                {
                    Email = "none@gmail.com",
                    Otp = "123",
                    NewPassword = "123"
                }));
        }

        //------------------------ GET ME --------------------------
        [Fact]
        public async Task GetMeAsync_ShouldThrow_WhenNotFound()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());
            await Assert.ThrowsAsync<Exception>(() => _service.GetMeAsync(1));
        }

        //------------------------ GET ACCOUNT BY ID --------------------------
        [Fact]
        public async Task GetAccountByIdAsync_ShouldThrow_WhenNotFound()
        {
            _accountRepo.Setup(x => x.Get()).Returns(new List<Account>().AsQueryable().BuildMock());
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetAccountByIdAsync(1));
        }
    }
}