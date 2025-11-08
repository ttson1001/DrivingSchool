using BEAPI.PaymentService.VnPay;
using Microsoft.AspNetCore.Mvc;
using TutorDrive.Dtos.common;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly VNPayService _vnpayService;
        private readonly ISystemConfigService _systemConfigService;

        public PaymentController(VNPayService vnpayService, ISystemConfigService systemConfigService)
        {
            _vnpayService = vnpayService;
            _systemConfigService = systemConfigService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CreateVNPayPayment([FromQuery] VnPayRequest request)
        {
            try
            {
                var paymentUrl = await _vnpayService.VNPayAsync(HttpContext, request);
                return Ok(new ResponseDto
                {
                    Message = "Tạo link thanh toán VNPay thành công.",
                    Data = new { paymentUrl }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Message = $"Lỗi khi tạo thanh toán: {ex.Message}"
                });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> VNPayReturn()
        {
            try
            {
                var result = await _vnpayService.VNPayReturnAsync(HttpContext);
                var data = result.Data as dynamic;
                string status = result.Message.Contains("thành công", StringComparison.OrdinalIgnoreCase)
                    ? "success"
                    : "fail";

                string? redirectBaseUrl = await _systemConfigService.GetValueAsync("PaymentReturnUrl");
                if (string.IsNullOrEmpty(redirectBaseUrl))
                    redirectBaseUrl = "https://localhost:4200/payment-result";

                string redirectUrl = $"{redirectBaseUrl}?status={status}&registrationId={data?.RegistrationId}&amount={data?.Amount}";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                string? redirectBaseUrl = await _systemConfigService.GetValueAsync("PaymentReturnUrl");
                if (string.IsNullOrEmpty(redirectBaseUrl))
                    redirectBaseUrl = "https://localhost:4200/payment-result";

                string redirectUrl = $"{redirectBaseUrl}?status=error&message={Uri.EscapeDataString(ex.Message)}";
                return Redirect(redirectUrl);
            }
        }


    }
}
