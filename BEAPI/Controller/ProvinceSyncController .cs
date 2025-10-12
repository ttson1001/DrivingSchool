using Azure;
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
    public class ProvinceSyncController : ControllerBase
    {
        private readonly IProvinceSyncService _syncService;

        public ProvinceSyncController(IProvinceSyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Đồng bộ danh sách Tỉnh/Thành", Description = "Đồng bộ toàn bộ Tỉnh/Thành từ nguồn dữ liệu bên ngoài")]
        [SwaggerResponse(200, "Đồng bộ thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SyncResponseExample))]
        public async Task<IActionResult> SyncProvinces()
        {
            var response = new ResponseDto();

            try
            {
                await _syncService.SyncProvincesAsync();

                response.Message = "Đã đồng bộ danh sách Tỉnh/Thành thành công!";
                response.Data = null;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi đồng bộ dữ liệu: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Đồng bộ danh sách Phường/Xã", Description = "Đồng bộ toàn bộ Phường/Xã từ nguồn dữ liệu bên ngoài")]
        [SwaggerResponse(200, "Đồng bộ thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SyncResponseExample))]
        public async Task<IActionResult> SyncAllWardsAsync()
        {
            var response = new ResponseDto();

            try
            {
                await _syncService.SyncAllWardsAsync();

                response.Message = "Đã đồng bộ danh sách Phường/Xã thành công!";
                response.Data = null;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi đồng bộ dữ liệu: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Lấy danh sách Tỉnh/Thành", Description = "Trả về danh sách tất cả Tỉnh/Thành")]
        [SwaggerResponse(200, "Lấy dữ liệu thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetAllProvincesResponseExample))]
        public async Task<IActionResult> GetAllProvinces()
        {

            var response = new ResponseDto();
            try
            {
                var provinces = await _syncService.GetAllProvincesAsync();

                response.Message = "Lấy dữ liệu thành công";
                response.Data = provinces;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"{ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{provinceCode}")]
        [SwaggerOperation(Summary = "Lấy danh sách Phường/Xã theo Tỉnh", Description = "Trả về danh sách Phường/Xã dựa trên mã Tỉnh")]
        [SwaggerResponse(200, "Lấy dữ liệu thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetWardsByProvinceResponseExample))]
        public async Task<IActionResult> GetWardsByProvinceCode(string provinceCode)
        {
            var response = new ResponseDto();
            try
            {
                var wards = await _syncService.GetWardsByProvinceCodeAsync(provinceCode);

                response.Message = "Lấy dữ liệu thành công";
                response.Data = wards;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"{ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
