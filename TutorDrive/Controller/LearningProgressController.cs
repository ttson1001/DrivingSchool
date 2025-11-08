using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LearningProgressController : ControllerBase
    {
        private readonly ILearningProgressService _service;

        public LearningProgressController(ILearningProgressService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Tạo tiến độ học cho học viên",
            Description = "Sinh tiến độ học cho học viên trong khóa học, gắn giáo viên phụ trách và danh sách section"
        )]
        [SwaggerResponse(200, "Tạo tiến độ học thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GenerateProgress([FromBody] GenerateProgressDto dto)
        {
            var response = new ResponseDto();

            try
            {
                await _service.GenerateProgressForCourseAsync(dto);
                response.Message = "Tạo tiến độ học thành công";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo tiến độ học: {ex.Message}";

                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
    Summary = "Cập nhật tiến độ học",
    Description = "Giáo viên cập nhật trạng thái hoàn thành, nhận xét và thời gian học của học viên"
)]
        [SwaggerResponse(200, "Cập nhật tiến độ học thành công", typeof(ResponseDto))]
        public async Task<IActionResult> UpdateProgress([FromBody] LearningProgressUpdateDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var accountIdClaim = User.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    response.Message = "Không tìm thấy thông tin giáo viên trong token";
                    return Unauthorized(response);
                }

                long accountId = long.Parse(accountIdClaim);

                await _service.UpdateProgressAsync(dto, accountId);

                response.Message = "Cập nhật tiến độ học thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật tiến độ học: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
    Summary = "Đổi giáo viên phụ trách hàng loạt",
    Description = "Thay đổi giáo viên cho toàn bộ tiến độ học của học viên trong một khóa học, chỉ đổi khi chưa hoàn thành"
)]
        public async Task<IActionResult> ChangeStaffForCourse([FromBody] ChangeStaffDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    response.Message = "Không tìm thấy thông tin giáo viên trong token";
                    return Unauthorized(response);
                }

                long accountId = long.Parse(accountIdClaim);

                await _service.ChangeStaffForCourseAsync(dto, accountId);

                response.Message = "Đổi giáo viên phụ trách thành công cho toàn bộ tiến độ";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi đổi giáo viên phụ trách: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{id}")]
        [SwaggerOperation(
    Summary = "Lấy chi tiết tiến độ học",
    Description = "Trả về thông tin chi tiết về tiến độ học của học viên bao gồm khóa học, phần học, giáo viên và trạng thái")]
        [SwaggerResponse(200, "Lấy tiến độ học thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetById(long id)
        {
            var response = new ResponseDto();

            try
            {
                var result = await _service.GetByIdAsync(id);
                response.Message = "Lấy tiến độ học thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy tiến độ học: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
    Summary = "Thống kê danh sách học viên của giáo viên",
    Description = "Trả về danh sách học viên đang học, đã hoàn thành và tổng số học viên của giáo viên hiện tại")]
        public async Task<IActionResult> GetTeacherStats()
        {
            var response = new ResponseDto();

            try
            {
                var accountIdClaim = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    response.Message = "Token không hợp lệ";
                    return Unauthorized(response);
                }

                long accountId = long.Parse(accountIdClaim);

                var result = await _service.GetTeacherOverviewAsync(accountId);

                response.Message = "Lấy thống kê thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy thống kê: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
           Summary = "Lấy danh sách tiến độ học giữa giáo viên và học viên",
           Description = "Trả về danh sách các phần học (LearningProgress) giữa giáo viên và học viên cụ thể"
       )]
        [SwaggerResponse(400, "Không tìm thấy hoặc lỗi xử lý dữ liệu")]
        public async Task<IActionResult> GetByTeacherAndStudent(
           [FromQuery] long? teacherId,
           [FromQuery] long? studentId)
        {
            var response = new ResponseDto();

            try
            {
                if (!teacherId.HasValue || !studentId.HasValue)
                {
                    response.Message = "Vui lòng cung cấp cả teacherId và studentId";
                    return BadRequest(response);
                }

                var result = await _service.GetByTeacherAndStudentAsync(teacherId.Value, studentId.Value);

                response.Message = "Lấy danh sách tiến độ học thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách tiến độ học: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
