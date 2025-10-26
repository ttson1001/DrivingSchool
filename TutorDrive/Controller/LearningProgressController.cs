using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GenerateProgress([FromQuery] long accountId, [FromQuery] long courseId)
        {
            try
            {
                await _service.GenerateProgressForCourseAsync(accountId, courseId);
                return Ok(new { message = "LearningProgress đã được tạo thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
