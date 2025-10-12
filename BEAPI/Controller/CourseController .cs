using BEAPI.Dtos.common;
using BEAPI.Dtos.Course;
using BEAPI.Extension.SwagerUi;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

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

        [HttpGet("[action]")]
        [SwaggerOperation(
     Summary = "Lấy danh sách course",
     Description = "Trả về toàn bộ danh sách course và thông tin các sections"
 )]
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
    }
}
