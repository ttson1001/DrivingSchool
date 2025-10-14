using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Common;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy tất cả vai trò", Description = "Trả về danh sách tất cả các vai trò trong hệ thống.")]
        [SwaggerResponse(200, "Lấy danh sách vai trò thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllRolesResponseExample))]
        public async Task<IActionResult> GetAll()
        {
            var data = await _roleService.GetAllRolesAsync();
            return Ok(new ResponseDto { Data = data, Message = "Lấy danh sách vai trò thành công." });
        }
    }
}
