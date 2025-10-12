using BEAPI.Dtos.common;
using BEAPI.Dtos.Vehicle;
using BEAPI.Extension.SwagerUi;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BEAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
           Summary = "Lấy danh sách xe",
           Description = "Trả về toàn bộ danh sách xe trong hệ thống"
       )]
        [SwaggerResponse(200, "Lấy danh sách xe thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllVehiclesResponseExample))]
        public async Task<IActionResult> GetAllVehicles()
        {
            var response = new ResponseDto();
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                response.Message = "Lấy danh sách xe thành công";
                response.Data = vehicles;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy dữ liệu: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
           Summary = "Tạo xe mới",
           Description = "Tạo một xe mới với thông tin được cung cấp"
       )]
        [SwaggerResponse(200, "Tạo xe thành công", typeof(ResponseDto))]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _vehicleService.CreateVehicleAsync(dto);
                response.Message = "Tạo xe thành công";
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo xe: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
           Summary = "Cập nhật xe",
           Description = "Cập nhật thông tin xe dựa trên ID"
       )]
        [SwaggerResponse(200, "Cập nhật xe thành công", typeof(ResponseDto))]
        public async Task<IActionResult> UpdateVehicle([FromBody] VehicleUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _vehicleService.UpdateVehicleAsync(dto);
                response.Message = "Cập nhật xe thành công";
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật xe: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
