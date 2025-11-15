using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TutorDrive.Dtos.common;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Lấy thống kê Dashboard Admin",
            Description = "Lấy số lượng tài khoản, học viên, giáo viên, doanh thu, đăng ký khóa học theo tháng."
        )]
        [SwaggerResponse(200, "Lấy thống kê thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Dashboard()
        {
            var response = new ResponseDto();

            try
            {
                var data = await _adminService.GetDashboardAsync();
                response.Message = "Lấy thống kê thành công";
                response.Data = data;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy thống kê: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
