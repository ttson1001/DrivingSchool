using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Vehicle;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleUsageHistoryController : ControllerBase
    {
        private readonly IVehicleUsageHistoryService _service;

        public VehicleUsageHistoryController(IVehicleUsageHistoryService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy toàn bộ lịch sử sử dụng xe")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllVehicleUsageHistoryResponseExample))]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDto();
            try
            {
                var list = await _service.GetAllAsync();
                response.Message = "Lấy danh sách thành công";
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{id}")]
        [SwaggerOperation(Summary = "Lấy lịch sử sử dụng theo ID")]
        [SwaggerResponse(200, "Lấy thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetVehicleUsageHistoryByIdResponseExample))]
        public async Task<IActionResult> GetById(long id)
        {
            var response = new ResponseDto();
            try
            {
                var dto = await _service.GetByIdAsync(id);
                response.Message = "Lấy dữ liệu thành công";
                response.Data = dto;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/vehicle/{vehicleId}")]
        [SwaggerOperation(Summary = "Lấy lịch sử theo VehicleId")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetVehicleUsageHistoryByVehicleResponseExample))]
        public async Task<IActionResult> GetAllByVehicleId(long vehicleId)
        {
            var response = new ResponseDto();
            try
            {
                var list = await _service.GetAllByVehicleIdAsync(vehicleId);
                response.Message = "Lấy danh sách thành công";
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy lịch sử theo AccountId từ JWT")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        public async Task<IActionResult> GetAllByAccountId()
        {
            var response = new ResponseDto();
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var list = await _service.GetAllByAccountIdAsync(accountId);

                response.Message = "Lấy danh sách thành công";
                response.Data = list;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Tạo lịch sử sử dụng xe mới")]
        [SwaggerResponse(200, "Tạo thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(CreateVehicleUsageHistoryResponseExample))]
        public async Task<IActionResult> Create([FromBody] VehicleUsageHistoryCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _service.CreateAsync(accountId, dto);

                response.Message = "Tạo lịch sử sử dụng xe thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Cập nhật lịch sử sử dụng xe")]
        [SwaggerResponse(200, "Cập nhật thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(UpdateVehicleUsageHistoryResponseExample))]
        public async Task<IActionResult> Update([FromBody] VehicleUsageHistoryUpdateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var accountId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                await _service.UpdateAsync(accountId, dto);

                response.Message = "Cập nhật lịch sử sử dụng xe thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
