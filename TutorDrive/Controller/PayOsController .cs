using Microsoft.AspNetCore.Mvc;
using TutorDrive.Dtos.common;
using TutorDrive.Services.Payment;
using TutorDrive.Services.IService;
using PayOS.Models.Webhooks;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayOsController : ControllerBase
    {
        private readonly PayOSService _payOsService;
        private readonly ISystemConfigService _systemConfigService;

        public PayOsController(PayOSService payOsService, ISystemConfigService systemConfigService)
        {
            _payOsService = payOsService;
            _systemConfigService = systemConfigService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CreatePayOsPayment(long registrationId)
        {
            try
            {
                var checkoutUrl = await _payOsService.CreatePaymentLinkAsync(registrationId);

                return Ok(new ResponseDto
                {
                    Message = "Tạo link thanh toán PayOS thành công.",
                    Data = new { checkoutUrl }
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

        [HttpPost("[action]")]
        public async Task<IActionResult> PayOsWebhook([FromBody] Webhook webhook)
        {
            try
            {
                var result = await _payOsService.HandleWebhookAsync(webhook);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto
                {
                    Message = "Lỗi xử lý webhook: " + ex.Message
                });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> PayOsReturn([FromQuery] long registrationId)
        {
            try
            {
                await _payOsService.PayOsReturnAsync(registrationId);
                string? redirectBaseUrl =
                    await _systemConfigService.GetValueAsync("PaymentReturnUrl")
                    ?? "https://localhost:4200/payment-result";

                string redirectUrl = $"{redirectBaseUrl}?status=success&registrationId={registrationId}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                string? redirectBaseUrl =
                    await _systemConfigService.GetValueAsync("PaymentReturnUrl")
                    ?? "https://localhost:4200/payment-result";

                string redirectUrl =
                    $"{redirectBaseUrl}?status=error&message={Uri.EscapeDataString(ex.Message)}";

                return Redirect(redirectUrl);
            }
        }
    }
}
