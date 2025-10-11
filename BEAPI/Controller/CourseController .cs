using BEAPI.Dtos.Course;
using BEAPI.Dtos.common;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("[action]")]
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
    }
}
