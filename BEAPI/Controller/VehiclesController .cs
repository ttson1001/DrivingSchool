using BEAPI.Dtos.Vehicle;
using BEAPI.Dtos.common;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var vehicle = await _vehicleService.CreateVehicleAsync(dto);
                response.Message = "Tạo xe thành công";
                response.Data = vehicle;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo xe: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateVehicle([FromBody] VehicleUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var vehicle = await _vehicleService.UpdateVehicleAsync(dto);
                if (vehicle == null)
                {
                    response.Message = "Xe không tồn tại";
                    return NotFound(response);
                }

                response.Message = "Cập nhật xe thành công";
                response.Data = vehicle;
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
