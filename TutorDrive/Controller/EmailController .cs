namespace TutorDrive.Controller
{
    using global::TutorDrive.Dtos.common;
    using global::TutorDrive.Services.IService;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;

    namespace TutorDrive.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class EmailController : ControllerBase
        {
            private readonly IEmailService _emailService;

            public EmailController(IEmailService emailService)
            {
                _emailService = emailService;
            }

            [HttpPost("[action]")]
            [SwaggerOperation(
                Summary = "Gửi email",
                Description = "Gửi một email đơn giản đến địa chỉ được chỉ định."
            )]
            [SwaggerResponse(200, "Gửi email thành công", typeof(ResponseDto))]
            public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
            {
                var response = new ResponseDto();
                try
                {
                    await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
                    response.Message = "Gửi email thành công";
                    response.Data = new { request.To };
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    response.Message = $"Lỗi khi gửi email: {ex.Message}";
                    return BadRequest(response);
                }
            }
        }
    }

}
