using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayOS.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Registration;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationFullService _service;

        public RegistrationController(IRegistrationFullService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
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
                var accountIdClaim = User.FindFirst("UserId")?.Value;
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

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Tìm kiếm đơn đăng ký học",
            Description = "Tìm kiếm đơn đăng ký học theo từ khóa, trạng thái và phân trang"
        )]
        [SwaggerResponse(200, "Lấy danh sách đăng ký thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(RegistrationListResponseExample))]
        public async Task<IActionResult> SearchRegistrations([FromQuery] RegistrationSearchDto filter)
        {
            var response = new ResponseDto();

            try
            {
                var result = await _service.SearchAsync(filter);

                response.Message = "Lấy danh sách đăng ký thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tìm kiếm đăng ký: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{accountId}")]
        [SwaggerOperation(
           Summary = "Lấy danh sách đơn đăng ký theo tài khoản",
           Description = "Trả về danh sách đăng ký học của học viên dựa trên AccountId (tự join qua StudentProfile). Có thể lọc theo trạng thái, từ khóa và ngày."
       )]
        [SwaggerResponse(200, "Lấy danh sách đăng ký thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetByAccount(long accountId, [FromQuery] RegistrationSearchDto filter)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _service.GetByAccountIdAsync(accountId, filter);

                response.Message = "Lấy danh sách đăng ký thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách đăng ký: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
          Summary = "Cập nhật trạng thái đơn đăng ký",
          Description = "Cho phép admin hoặc giáo viên thay đổi trạng thái đơn đăng ký của học viên (Pending, Approved, Rejected, Cancelled...)."
      )]
        [SwaggerResponse(200, "Cập nhật trạng thái thành công", typeof(ResponseDto))]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRegistrationStatusDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _service.UpdateStatusAsync(dto);

                response.Message = "Cập nhật trạng thái đơn đăng ký thành công.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật trạng thái: {ex.Message}";
                return BadRequest(response);
            }
        }

    }
}
