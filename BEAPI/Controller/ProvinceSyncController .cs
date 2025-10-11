using Azure;
using BEAPI.Dtos.common;
using BEAPI.Dtos.location;
using BEAPI.Dtos.Location;
using BEAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ProvinceApiResponse>> GetAllProvinces()
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
        public async Task<ActionResult<WardApiResponseDto>> GetWardsByProvinceCode(string provinceCode)
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
