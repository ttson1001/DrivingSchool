
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Model;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;


namespace BEAPI.PaymentService.VnPay
{
    public class VNPayService
    {
        private readonly VnPaySettings _settings;
        private readonly IRepository<Registration> _repository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly ILearningProgressService _learningProgressService;
        private readonly IRepository<InstructorProfile> _staffRepository;

        public VNPayService(
            IRepository<Registration> registrationRepository,
            IRepository<Transaction> transactionRepository,
            ILearningProgressService learningProgressService,
            IRepository<InstructorProfile> staffRepository,
            IOptions<VnPaySettings> options)
        {
            _settings = options.Value;
            _repository = registrationRepository;
            _transactionRepository = transactionRepository;
            _learningProgressService = learningProgressService;
            _staffRepository = staffRepository;
        }

        public async Task<string> VNPayAsync(HttpContext context, VnPayRequest vnPayRequest)
        {
            var createdDate = DateTime.Now;
            var MGT = DateTime.Now.Ticks;

            var courseAmmount = await _repository.Get().Include(x => x.Course)
                                                 .Where(x => x.Id == vnPayRequest.RegistrationId).Select(x => x.Price).FirstAsync();
            var vnpAmmount = (long)(courseAmmount * 100);

            var orderInfo = $"RegistrationId={vnPayRequest.RegistrationId}";

            VnPayLib vnpay = new VnPayLib();
            vnpay.AddRequestData("vnp_Version", VnPayLib.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _settings.TmCode);
            vnpay.AddRequestData("vnp_Amount", vnpAmmount.ToString(CultureInfo.InvariantCulture));
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", MGT.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);
            return paymentUrl;
        }

        public async Task<ResponseDto> VNPayReturnAsync(HttpContext context)
        {
            var query = context.Request.Query;
            var vnpay = new VnPayLib();

            foreach (var (key, value) in query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    vnpay.AddResponseData(key, value);
            }

            string vnp_SecureHash = query["vnp_SecureHash"];
            bool isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, _settings.HashSecret);

            if (!isValidSignature)
                return new ResponseDto { Message = "Chữ ký VNPay không hợp lệ.", Data = null };

            string orderInfo = WebUtility.UrlDecode(vnpay.GetResponseData("vnp_OrderInfo"));
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string txnRef = vnpay.GetResponseData("vnp_TxnRef");
            long amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;

            var match = Regex.Match(orderInfo, @"RegistrationId=(\d+)");
            if (!match.Success)
                return new ResponseDto { Message = "Không tìm thấy mã đăng ký trong OrderInfo.", Data = null };

            long registrationId = long.Parse(match.Groups[1].Value);
            bool isSuccess = responseCode == "00" && transactionStatus == "00";

            var registration = await _repository.Get()
                .Include(r => r.Course)
                .Include(r => r.StudentProfile).ThenInclude(sp => sp.Account)
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration == null)
                return new ResponseDto { Message = "Không tìm thấy đơn đăng ký.", Data = null };

            var transaction = new Transaction
            {
                Amount = amount,
                UserId = registration.StudentProfile.Account.Id,
                PaymentMethod = "VNPay",
                RegistrationId = registration.Id,
                PaymentStatus = PaymentStatus.Paid
            };

            await _transactionRepository.AddAsync(transaction);
            if (isSuccess)
            {
                // Cập nhật trạng thái đăng ký
                registration.Status = RegistrationStatus.Paid;
                registration.Note = $"Đã thanh toán VNPay ({txnRef}) - {DateTime.Now:dd/MM/yyyy HH:mm}";
                _repository.Update(registration);

                // Tạm chọn giáo viên bất kỳ (có thể chọn theo ExperienceYears)
                var teacher = await _staffRepository.Get()
                    .OrderByDescending(s => s.ExperienceYears)
                    .FirstOrDefaultAsync();

                if (teacher == null)
                    throw new Exception("Không tìm thấy giáo viên khả dụng.");

                // Gọi service tạo tiến trình học theo lịch đăng ký (thứ, ngày bắt đầu)
                var dto = new GenerateProgressDto
                {
                    StudentId = registration.StudentProfileId,
                    TeacherId = teacher.Id,
                    CourseId = registration.CourseId,
                    RegisterId = registrationId,
                    StartDate = registration.StartDateTime
                };

                await _learningProgressService.GenerateProgressForCourseAsync(dto);
            }

            await _transactionRepository.SaveChangesAsync();
            await _repository.SaveChangesAsync();

            return new ResponseDto
            {
                Message = isSuccess ? "Thanh toán thành công." : "Thanh toán thất bại.",
                Data = new
                {
                    RegistrationId = registration.Id,
                    TransactionId = txnRef,
                    Amount = amount,
                    PaymentStatus = transaction.PaymentStatus.ToString(),
                    RegistrationStatus = registration.Status.ToString()
                }
            };
        }
    }
}
