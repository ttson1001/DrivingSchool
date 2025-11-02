
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Net;
using TutorDrive.Entities;
using TutorDrive.Model;
using TutorDrive.Repositories;


namespace BEAPI.PaymentService.VnPay
{
    public class VNPayService
    {
        private readonly VnPaySettings _settings;
        private readonly IRepository<Registration> _repository;
        private readonly IAccount _userService;

        public VNPayService(IAccount userService, IRepository<Registration> repository, IOptions<VnPaySettings> options)
        {
            _settings = options.Value;
            _repository = repository;
            _userService = userService;
        }

        public async Task<string> VNPayAsync(HttpContext context, VnPayRequest vnPayRequest)
        {
            var createdDate = DateTime.Now;
            var MGT = DateTime.Now.Ticks;

            var courseAmmount = await _repository.Get().Include(x => x.Course)
                                                 .Where(x => x.Id == vnPayRequest.RegistrationId).Select(x => x.Course.Price).FirstAsync();
            var vnpAmmount = courseAmmount * 100;

            var orderInfo = $"RegistrationId={vnPayRequest.RegistrationId}";

            VnPayLib vnpay = new VnPayLib();
            vnpay.AddRequestData("vnp_Version", VnPayLib.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _settings.TmCode);
            vnpay.AddRequestData("vnp_Amount", vnpAmmount.ToString());
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", WebUtility.UrlEncode(orderInfo));
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", MGT.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);
            return paymentUrl;
        }
    }
}
