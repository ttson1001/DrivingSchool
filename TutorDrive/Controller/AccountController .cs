using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Account;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Staff.TutorDrive.Dtos.Accounts;
using TutorDrive.Exceptions;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới", Description = "Tạo tài khoản người dùng mới.")]
        [SwaggerResponse(200, "Đăng ký thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(RegisterResponseExample))]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest dto)
        {
            var response = new ResponseDto();
            try
            {
                await _accountService.RegisterAsync(dto);
                return Ok(new ResponseDto { Data = null, Message = "Đăng ký thành công." });
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                if (ex.Message == ExceptionConstant.UserAlreadyExists)
                    return Conflict(response);

                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            await _accountService.ForgotPasswordAsync(dto);
            return Ok(new { message = "OTP đã được gửi đến email." });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
        {
            await _accountService.ResetPasswordAsync(dto);
            return Ok(new { message = "Đặt lại mật khẩu thành công." });
        }

        [HttpPut("status/{id}")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái tài khoản (Admin)")]
        public async Task<IActionResult> UpdateStatus(long id, [FromQuery] bool isActive)
        {
            var response = new ResponseDto();

            try
            {
                await _accountService.SetStatusAsync(id, isActive);

                response.Message = isActive
                    ? "Mở khóa tài khoản thành công"
                    : "Khóa tài khoản thành công";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Đăng nhập tài khoản", Description = "Trả về JWT token khi đăng nhập thành công.")]
        [SwaggerResponse(200, "Đăng nhập thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(LoginResponseExample))]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var data = await _accountService.LoginAsync(dto);
                return Ok(new ResponseDto { Data = data, Message = "Đăng nhập thành công." });
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
     Summary = "Tìm kiếm tài khoản",
     Description = "Lọc tài khoản theo keyword (email hoặc họ tên), roleId và có hỗ trợ phân trang.")]
        [SwaggerResponse(200, "Trả về danh sách tài khoản có phân trang", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SearchAccountsResponseExample))]
        public async Task<IActionResult> SearchAccounts([FromQuery] string? keyword, [FromQuery] long? roleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var data = await _accountService.SearchAccountsAsync(keyword, roleId, page, pageSize);
                return Ok(new ResponseDto
                {
                    Data = data,
                    Message = "Lấy danh sách tài khoản thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto
                {
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                });
            }
        }


        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Xem chi tiết tài khoản", Description = "Lấy thông tin chi tiết của tài khoản theo ID.")]
        [SwaggerResponse(200, "Trả về chi tiết tài khoản", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAccountByIdResponseExample))]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data = await _accountService.GetAccountByIdAsync(id);
                return Ok(new ResponseDto { Data = data, Message = "Lấy chi tiết tài khoản thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto { Message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Cập nhật tài khoản", Description = "Chỉnh sửa thông tin tài khoản (họ tên, mật khẩu, vai trò).")]
        [SwaggerResponse(200, "Cập nhật tài khoản thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(UpdateAccountResponseExample))]
        public async Task<IActionResult> UpdateAccount(long id, [FromBody] AccountUpdateDto dto)
        {
            try
            {
                var data = await _accountService.UpdateAccountAsync(id, dto);
                return Ok(new ResponseDto { Data = data, Message = "Cập nhật tài khoản thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseDto { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDto dto)
        {
            try
            {
                await _accountService.CreateAccountAsync(dto);
                return Ok(new { message = "Tạo tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var accountIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(accountIdStr))
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(accountIdStr, out var accountId))
                    return BadRequest(new { message = "ID tài khoản không hợp lệ" });

                var me = await _accountService.GetMeAsync(accountId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin tài khoản thành công",
                    data = me
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

        [HttpGet("[action]/{userId:long}")]
        public async Task<IActionResult> GetAccountInfo(long userId)
        {
            try
            {
                var me = await _accountService.GetMeAsync(userId);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin tài khoản thành công",
                    data = me
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileDto dto)
        {
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var updatedProfile = await _accountService.UpdateAsync(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật hồ sơ học viên thành công",
                    data = updatedProfile
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

        [HttpPut("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateInstructorProfileAsync([FromBody] UpdateInstructorProfileDto dto)
        {
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var updatedProfile = await _accountService.UpdateInstructorProfileAsync(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật hồ sơ học viên thành công",
                    data = updatedProfile
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
        public async Task<IActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _accountService.GetAllInstructorsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách giáo viên thành công",
                    data = instructors
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

        [HttpPut("[action]")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest dto)
        {
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _accountService.ChangePasswordAsync(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công"
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
