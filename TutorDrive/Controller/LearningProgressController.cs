using Microsoft.AspNetCore.Mvc;
using PayOS.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Services;
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
    Summary = "Lấy danh sách tiến trình học của học viên theo giảng viên",
    Description = "Group theo học viên. isCompleted = true/false hoặc null để lấy tất cả."
)]
        public async Task<IActionResult> GetInstructorLearningProgress([FromQuery] bool? isCompleted)
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

                var result = await _service.GetByInstructorGroupedAsync(accountId, isCompleted);

                response.Message = "Lấy danh sách tiến trình học theo giảng viên thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy dữ liệu: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
      Summary = "Admin xem tiến trình học của tất cả học viên",
      Description = "Group theo Course → Student → Progress. Có thể lọc theo isCompleted (true/false). Không truyền sẽ lấy tất cả."
  )]
        [SwaggerResponse(200, "Lấy danh sách tiến trình học thành công", typeof(ResponseDto))]
        [SwaggerResponse(401, "Token không hợp lệ")]
        [SwaggerResponse(400, "Lỗi khi lấy dữ liệu")]
        public async Task<IActionResult> GetAllLearningProgress([FromQuery] bool? isCompleted)
        {
            var response = new ResponseDto();

            try
            {
                var result = await _service.GetAdminLearningProgressAsync(isCompleted);

                response.Data = result;
                response.Message = "Lấy danh sách tiến trình học thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy dữ liệu: {ex.Message}";
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
    Summary = "Lấy danh sách tiến trình học của học viên",
    Description = "Có thể lọc theo trạng thái hoàn thành (isCompleted). Nếu không truyền thì lấy tất cả."
)]
        [SwaggerResponse(200, "Lấy danh sách tiến trình học thành công", typeof(ResponseDto))]
        [SwaggerResponse(401, "Token không hợp lệ")]
        [SwaggerResponse(400, "Lỗi khi lấy dữ liệu")]
        public async Task<IActionResult> GetMyLearningProgress([FromQuery] bool? isCompleted)
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

                var result = await _service.GetByStudentGroupedAsync(accountId, isCompleted);

                response.Message = "Lấy danh sách tiến trình học thành công";
                response.Data = result;

                return Ok(response);
            }
            catch(NotFoundException ex)
            {
                return NoContent();
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách tiến trình học: {ex.Message}";
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

        [HttpGet("[action]")]
        [SwaggerOperation(
    Summary = "Danh sách khóa học đã hoàn thành 100%",
    Description = "Lấy danh sách khóa học mà học viên đã hoàn tất toàn bộ tiến độ học"
)]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetCompletedCourses()
        {
            var response = new ResponseDto();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var result = await _service.GetCompletedCoursesByStudentAsync(accountId);

                response.Message = "Lấy danh sách khóa học hoàn thành thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy khóa học hoàn thành: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]/{studentId}")]
        public async Task<IActionResult> UpdateTraining(long studentId, [FromBody] UpdateStudentTrainingDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _service.UpdateStudentTrainingAsync(studentId, dto, accountId);

                response.Message = "Cập nhật thông tin đào tạo thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật: {ex.Message}";
                return BadRequest(response);
            }
        }

    }
}
