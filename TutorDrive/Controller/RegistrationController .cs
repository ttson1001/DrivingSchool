using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Registration;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationFullService _service;

        public RegistrationController(IRegistrationFullService service)
        {
            _service = service;
        }

        [HttpPost("full")]
        [SwaggerOperation(
            Summary = "Đăng ký khóa học đầy đủ",
            Description = "Thực hiện đăng ký khóa học mới, kèm thông tin hồ sơ và upload file CCCD (mặt trước/mặt sau)"
        )]
        [SwaggerResponse(200, "Đăng ký thành công", typeof(ResponseDto))]
        public async Task<IActionResult> RegisterFull([FromBody] RegistrationFullCreateDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (accountIdClaim == null)
                {
                    response.Message = "Không tìm thấy thông tin tài khoản.";
                    return Unauthorized(response);
                }

                var accountId = long.Parse(accountIdClaim);

                await _service.RegisterFullAsync(accountId, dto);

                response.Message = "Đăng ký khóa học thành công!";
                response.Data = null;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi đăng ký khóa học: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
