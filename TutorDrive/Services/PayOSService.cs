using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Model;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services.Payment
{
    public class PayOSService
    {
        private readonly PayOSClient _client;
        private readonly PayOsSettings _settings;
        private readonly IRepository<Registration> _registrationRepo;
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly ILearningProgressService _learningProgressService;
        private readonly IRepository<InstructorProfile> _instructorRepo;

        public PayOSService(
            IRepository<Registration> registrationRepo,
            IRepository<Transaction> transactionRepo,
            ILearningProgressService learningProgressService,
            IRepository<InstructorProfile> instructorRepo,
            IOptions<PayOsSettings> options)
        {
            _settings = options.Value;

            _client = new PayOSClient(new PayOSOptions
            {
                ClientId = _settings.ClientId,
                ApiKey = _settings.ApiKey,
                ChecksumKey = _settings.ChecksumKey
            });

            _registrationRepo = registrationRepo;
            _transactionRepo = transactionRepo;
            _learningProgressService = learningProgressService;
            _instructorRepo = instructorRepo;
        }

        public async Task<string> CreatePaymentLinkAsync(long registrationId)
        {
            var registration = await _registrationRepo.Get()
                .Include(r => r.Course)
                .Include(r => r.StudentProfile).ThenInclude(sp => sp.Account)
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration == null)
                throw new Exception("Không tìm thấy đơn đăng ký.");

            var request = new CreatePaymentLinkRequest
            {
                OrderCode = registrationId,
                Amount = (long)registration.Price,
                Description = $"Thanh toán khóa học",
                ReturnUrl = _settings.ReturnUrl + $"?registrationId={registrationId}",
                CancelUrl = _settings.CancelUrl
            };

            var response = await _client.PaymentRequests.CreateAsync(request);

            return response.CheckoutUrl;
        }

        public async Task<ResponseDto> HandleWebhookAsync(Webhook webhook)
        {
            var webhookData = await _client.Webhooks.VerifyAsync(webhook);

            if (webhookData == null)
                return new ResponseDto { Message = "Webhook không hợp lệ (Sai signature)." };

            long registrationId = webhookData.OrderCode;
            decimal paidAmount = webhookData.Amount;

            var registration = await _registrationRepo.Get()
                .Include(r => r.Course)
                .Include(r => r.StudentProfile).ThenInclude(sp => sp.Account)
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration == null)
                return new ResponseDto { Message = "Không tìm thấy đơn đăng ký." };

            bool success = webhookData.Code == "00";

            var transaction = new Transaction
            {
                Amount = paidAmount,
                UserId = registration.StudentProfile.Account.Id,
                PaymentMethod = "PayOS",
                RegistrationId = registration.Id,
                PaymentStatus = PaymentStatus.Paid
            };

            await _transactionRepo.AddAsync(transaction);

            if (success)
            {
                registration.Status = RegistrationStatus.Paid;
                registration.Note = $"Thanh toán PayOS – {DateTime.UtcNow:dd/MM/yyyy HH:mm}";
                _registrationRepo.Update(registration);

                var teacher = await _instructorRepo.Get()
                    .OrderByDescending(i => i.ExperienceYears)
                    .FirstOrDefaultAsync();

                if (teacher != null)
                {
                    await _learningProgressService.GenerateProgressForCourseAsync(
                        new GenerateProgressDto
                        {
                            StudentId = registration.StudentProfileId,
                            TeacherId = teacher.Id,
                            CourseId = registration.CourseId,
                            RegisterId = registration.Id,
                            StartDate = registration.StartDateTime
                        }
                    );
                }
            }

            await _transactionRepo.SaveChangesAsync();
            await _registrationRepo.SaveChangesAsync();

            return new ResponseDto
            {
                Message = success ? "Thanh toán thành công." : "Thanh toán thất bại.",
                Data = new
                {
                    registration.Id,
                    transaction.PaymentStatus,
                    registration.Status
                }
            };
        }
    }
}
