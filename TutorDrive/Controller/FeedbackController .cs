using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Exam;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services;
using TutorDrive.Services.IService;
namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Tạo phản hồi mới",
            Description = "Sinh viên gửi phản hồi đến nhân viên/tutor"
        )]
        [SwaggerResponse(200, "Tạo phản hồi thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(CreateFeedbackResponseExample))]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _feedbackService.CreateAsync(dto, userId);
                response.Message = "Tạo phản hồi thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo phản hồi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Lấy danh sách phản hồi",
            Description = "Trả về toàn bộ phản hồi của sinh viên"
        )]
        [SwaggerResponse(200, "Lấy danh sách phản hồi thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllFeedbacksResponseExample))]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var response = new ResponseDto();
            try
            {
                var items = await _feedbackService.GetAllAsync();
                response.Message = "Lấy danh sách phản hồi thành công";
                response.Data = items;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách phản hồi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{id}")]
        [SwaggerOperation(
            Summary = "Lấy phản hồi theo ID",
            Description = "Trả về thông tin chi tiết của một phản hồi"
        )]
        [SwaggerResponse(200, "Lấy phản hồi thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetFeedbackByIdResponseExample))]
        public async Task<IActionResult> GetFeedbackById(long id)
        {
            var response = new ResponseDto();
            try
            {
                var item = await _feedbackService.GetByIdAsync(id);
                response.Message = "Lấy phản hồi thành công";
                response.Data = item;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy phản hồi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Tìm kiếm phản hồi",
            Description = "Tìm phản hồi theo từ khóa trong bình luận, có phân trang"
        )]
        [SwaggerResponse(200, "Lấy danh sách phản hồi thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SearchFeedbacksResponseExample))]
        public async Task<IActionResult> SearchFeedbacks(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _feedbackService.SearchAsync(keyword, page, pageSize);
                response.Message = "Lấy danh sách phản hồi thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tìm kiếm phản hồi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]/{id}")]
        [SwaggerOperation(
            Summary = "Cập nhật phản hồi",
            Description = "Cập nhật nội dung hoặc điểm đánh giá của phản hồi"
        )]
        [SwaggerResponse(200, "Cập nhật phản hồi thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(UpdateFeedbackResponseExample))]
        public async Task<IActionResult> UpdateFeedback(long id, [FromBody] FeedbackUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _feedbackService.UpdateAsync(id, dto);
                response.Message = "Cập nhật phản hồi thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật phản hồi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Top giáo viên có đánh giá cao nhất",
            Description = "Trả về danh sách N giáo viên có điểm trung bình cao nhất theo phản hồi học viên"
        )]
        [SwaggerResponse(200, "Danh sách top giáo viên", typeof(IEnumerable<TopTeacherDto>))]
        public async Task<IActionResult> GetTopTeachers([FromQuery] int top = 5)
        {
            var response = new ResponseDto();

            try
            {
                var result = await _feedbackService.GetTopTeachersAsync(top);

                response.Message = "Lấy danh sách giáo viên thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách giáo viên: {ex.Message}";
                response.Data = null;
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHomepageAsync()
        {
            try
            {
                var result = await _feedbackService.GetHomepageAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu homepage thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var history = await _feedbackService.GetHistoryAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy lịch sử phản hồi thành công",
                    data = history
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}
