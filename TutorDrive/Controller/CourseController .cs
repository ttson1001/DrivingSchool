using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Course;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using static TutorDrive.Extension.SwagerUi.SearchCoursesResponseExample;

namespace BEAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Tạo course mới",
            Description = "Tạo một course cùng các sections đi kèm"
        )]
        [SwaggerResponse(200, "Tạo course cùng sections thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(CreateCourseResponseExample))]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _courseService.CreateCourseWithSectionsAsync(dto);
                response.Message = "Tạo course cùng sections thành công";
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo course: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
             Summary = "Cập nhật khóa học",
             Description = "Cập nhật thông tin khóa học và danh sách sections (có thể thêm/sửa/xóa sections)"
         )]
        [SwaggerResponse(200, "Cập nhật khóa học thành công", typeof(ResponseDto))]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _courseService.UpdateCourseWithSectionsAsync(dto);

                response.Message = "Cập nhật khóa học thành công";
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật khóa học: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
         Summary = "Lấy danh sách course",
         Description = "Trả về toàn bộ danh sách course và thông tin các sections")]
        [SwaggerResponse(200, "Lấy danh sách course thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllCoursesResponseExample))]
        public async Task<IActionResult> GetAllCourses()
        {
            var response = new ResponseDto();
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                response.Message = "Lấy danh sách course thành công";
                response.Data = courses;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách course: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
           Summary = "Tìm kiếm khóa học",
           Description = "Tìm kiếm khóa học theo từ khóa và phân trang"
       )]
        [SwaggerResponse(200, "Lấy danh sách khóa học thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SearchCoursesResponseExample))]
        public async Task<IActionResult> SearchCourses(
           [FromQuery] string? keyword,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDto();

            try
            {
                var result = await _courseService.SearchCoursesAsync(keyword, page, pageSize);

                response.Message = "Lấy danh sách khóa học thành công";
                response.Data = result;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách khóa học: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
