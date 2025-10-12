using BEAPI.Dtos.Vehicle;
using BEAPI.Dtos.common;
using BEAPI.Extension.SwagerUi;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BEAPI.Controller
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

        [HttpGet("[action]/account/{accountId}")]
        [SwaggerOperation(Summary = "Lấy lịch sử theo AccountId")]
        [SwaggerResponse(200, "Lấy danh sách thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetVehicleUsageHistoryByAccountResponseExample))]
        public async Task<IActionResult> GetAllByAccountId(long accountId)
        {
            var response = new ResponseDto();
            try
            {
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
                await _service.CreateAsync(dto);
                response.Message = "Tạo lịch sử thành công";
                response.Data = null;
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
                await _service.UpdateAsync(dto);
                response.Message = "Cập nhật thành công";
                response.Data = null;
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
