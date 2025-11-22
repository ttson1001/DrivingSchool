using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.RegistrationExam;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationExamController : ControllerBase
    {
        private readonly IRegistrationExamService _service;

        public RegistrationExamController(IRegistrationExamService service)
        {
            _service = service;
        }

        private long GetAccountId()
        {
            var accountIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(accountIdStr))
                throw new Exception("Không tìm thấy thông tin người dùng trong token");

            if (!long.TryParse(accountIdStr, out var id))
                throw new Exception("ID người dùng không hợp lệ");

            return id;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Nộp hồ sơ dự thi bằng lái",
            Description = "Học viên upload CCCD, ảnh thẻ, giấy khám sức khỏe và đơn đề nghị sát hạch.")]
        [SwaggerResponse(200, "Nộp hồ sơ thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Submit([FromBody] RegistrationExamCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var accountId = GetAccountId();
                var data = await _service.SubmitAsync(dto, accountId);

                response.Message = "Nộp hồ sơ thành công";
                response.Data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Xem danh sách hồ sơ đã nộp của học viên")]
        public async Task<IActionResult> MyRegistrations()
        {
            var response = new ResponseDto();
            try
            {
                var accountId = GetAccountId();
                var data = await _service.GetMyRegistrationsAsync(accountId);

                response.Message = "Lấy hồ sơ thành công";
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Xem chi tiết hồ sơ thi theo Id")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = new ResponseDto();
            try
            {
                var data = await _service.GetByIdAsync(id);

                if (data == null)
                {
                    response.Message = "Không tìm thấy hồ sơ.";
                    return NotFound(response);
                }

                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
    Summary = "Học viên cập nhật lại hồ sơ đăng ký thi",
    Description = "Dùng khi hồ sơ bị sai hoặc bị từ chối, chỉ sửa được khi đang Pending hoặc Rejected.")]
        public async Task<IActionResult> UpdateMyRegistration([FromForm] RegistrationExamUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var accountId = GetAccountId();
                var data = await _service.UpdateByStudentAsync(dto, accountId);

                response.Message = "Cập nhật hồ sơ thành công.";
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Tìm kiếm / lọc hồ sơ dự thi có phân trang")]
        public async Task<IActionResult> Search([FromQuery] RegistrationExamSearchRequest request)
        {
            var response = new ResponseDto();
            try
            {
                var data = await _service.SearchAsync(request);

                response.Data = data;
                response.Message = "Lấy danh sách thành công";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái hồ sơ: Duyệt hoặc Từ Chối")]
        public async Task<IActionResult> UpdateStatus([FromBody] RegistrationExamStatusUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var adminId = GetAccountId();

                await _service.UpdateStatusAsync(dto, adminId);

                response.Message = "Cập nhật trạng thái thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
