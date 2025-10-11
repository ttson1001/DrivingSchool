namespace BEAPI.Controller
{
    using BEAPI.Dtos.account;
    using BEAPI.Dtos.common;
    using BEAPI.Exceptions;
    using global::BEAPI.Services.IService;
    using Microsoft.AspNetCore.Mvc;

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
