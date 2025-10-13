using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Vehicle;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace TutorDrive.Controller
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

        [HttpGet("[action]")]
        [SwaggerOperation(
           Summary = "Tìm kiếm và phân trang danh sách xe",
           Description = "Cho phép tìm kiếm theo biển số, hãng, hoặc mẫu xe. Có hỗ trợ phân trang."
       )]
        [SwaggerResponse(200, "Lấy danh sách xe thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SearchVehiclesResponseExample))]
        public async Task<IActionResult> SearchVehicles([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _vehicleService.SearchVehiclesAsync(keyword, page, pageSize);
                response.Message = "Lấy danh sách xe thành công";
                response.Data = result;
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
