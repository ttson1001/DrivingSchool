using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Complaint;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _service;

        public ComplaintController(IComplaintService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy toàn bộ danh sách khiếu nại (Admin)")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDto();
            try
            {
                var list = await _service.GetAllAsync();
                response.Message = "Lấy danh sách thành công";
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy danh sách khiếu nại của chính người dùng")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetMy()
        {
            var response = new ResponseDto();
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var list = await _service.GetMyHistoryAsync(accountId);
                response.Message = "Lấy danh sách thành công";
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Người dùng tạo khiếu nại")]
        [SwaggerResponse(200, "Tạo khiếu nại thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Create([FromBody] ComplaintCreateDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _service.CreateAsync(accountId, dto);

                response.Message = "Gửi khiếu nại thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpDelete("[action]/{id}")]
        [SwaggerOperation(Summary = "Xóa khiếu nại của chính người dùng")]
        [SwaggerResponse(200, "Xóa thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Delete(long id)
        {
            var response = new ResponseDto();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { message = "ID người dùng không hợp lệ" });

                await _service.DeleteAsync(accountId, id);

                response.Message = "Xóa khiếu nại thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Admin phản hồi khiếu nại")]
        [SwaggerResponse(200, "Phản hồi thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Reply([FromBody] ComplaintReplyDto dto)
        {
            var response = new ResponseDto();

            try
            {
                await _service.ReplyAsync(dto);
                response.Message = "Phản hồi khiếu nại thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
