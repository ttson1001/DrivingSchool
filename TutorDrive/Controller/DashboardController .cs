using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/instructor/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IInstructorDashboardService _dashboardService;

        public DashboardController(IInstructorDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInstructorDashboardJwtAsync()
        {
            try
            {
                var userIdStr = User.FindFirstValue("UserId");

                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Không tìm thấy thông tin người dùng trong token"
                    });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID người dùng trong token không hợp lệ"
                    });

                var dashboard = await _dashboardService.GetDashboardAsync(accountId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu dashboard giảng viên thành công",
                    data = dashboard
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{accountId:long}")]
        public async Task<IActionResult> GetInstructorDashboardByIdAsync(long accountId)
        {
            try
            {
                var dashboard = await _dashboardService.GetDashboardAsync(accountId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu dashboard giảng viên thành công",
                    data = dashboard
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
    }
}
