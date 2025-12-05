using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackClusterController : ControllerBase
    {
        private readonly IFeedbackClusterService _service;

        public FeedbackClusterController(IFeedbackClusterService service)
        {
            _service = service;
        }

        // ============================
        // Lấy tất cả cluster (Admin)
        // ============================
        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy toàn bộ lịch sử AI clustering (Admin)")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDto();

            try
            {
                var data = await _service.GetAllClustersAsync();
                response.Message = "Lấy danh sách thành công";
                response.Data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        // ============================
        // Tạo cluster mới từ feedback
        // ============================
        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Phân nhóm feedback bằng AI và lưu kết quả")]
        [SwaggerResponse(200, "Phân nhóm thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Cluster()
        {
            var response = new ResponseDto();

            try
            {
                var result = await _service.ProcessFeedbackAsync();

                response.Message = "Phân nhóm thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        // ============================
        // Xóa toàn bộ cluster
        // ============================
        [HttpDelete("[action]")]
        [SwaggerOperation(Summary = "Xóa toàn bộ lịch sử clustering")]
        [SwaggerResponse(200, "Xóa thành công", typeof(ResponseDto))]
        public async Task<IActionResult> DeleteAll()
        {
            var response = new ResponseDto();

            try
            {
                await _service.DeleteAllAsync();
                response.Message = "Đã xóa toàn bộ lịch sử clustering";
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
