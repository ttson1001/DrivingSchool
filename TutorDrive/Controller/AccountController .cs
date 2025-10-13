namespace TutorDrive.Controller
{
    using TutorDrive.Dtos.account;
    using TutorDrive.Dtos.common;
    using TutorDrive.Exceptions;
    using TutorDrive.Extension.SwagerUi;
    using global::TutorDrive.Services.IService;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
 
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới", Description = "Tạo tài khoản người dùng mới.")]
        [SwaggerResponse(200, "Đăng ký thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(RegisterResponseExample))]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest dto)
        {
            var response = new ResponseDto();

            try
            {
                await _accountService.RegisterAsync(dto);
                return Ok(new ResponseDto { Data = null, Message = "Đăng kí thành công" });
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
        [SwaggerOperation(
            Summary = "Đăng nhập tài khoản",
            Description = "Trả về JWT token khi đăng nhập thành công."
        )]
        [SwaggerResponseExample(200, typeof(LoginResponseExample))]
        [SwaggerResponse(200, "Đăng nhập thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = new ResponseDto();

            try
            {
                return Ok(new ResponseDto { Data = await _accountService.LoginAsync(dto), Message = "Đăng kí thành công" });
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

                if (ex.Message == ExceptionConstant.UserAlreadyExists)
                    return Conflict(response);

                return BadRequest(response);
            }
        }
    }
}
