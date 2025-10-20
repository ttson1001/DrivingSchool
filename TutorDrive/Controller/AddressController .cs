using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.Address;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
using TutorDrive.Dtos.common;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Lấy danh sách địa chỉ",
            Description = "Trả về danh sách tất cả địa chỉ trong hệ thống."
        )]
        [SwaggerResponse(200, "Lấy danh sách địa chỉ thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(AddressResponseExample))]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDto();
            try
            {
                var list = await _addressService.GetAllAsync();
                response.Message = "Lấy danh sách địa chỉ thành công";
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách địa chỉ: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Tạo mới địa chỉ",
            Description = "Tạo một địa chỉ mới trong hệ thống."
        )]
        [SwaggerResponse(200, "Tạo địa chỉ thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Create([FromBody] AddressDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _addressService.CreateAddressAsync(dto);
                response.Message = "Tạo địa chỉ thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo địa chỉ: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
            Summary = "Cập nhật địa chỉ",
            Description = "Cập nhật thông tin của địa chỉ theo ID."
        )]
        [SwaggerResponse(200, "Cập nhật địa chỉ thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Update([FromBody] UpdateAddressDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _addressService.UpdateAddressAsync(dto);
                response.Message = "Cập nhật địa chỉ thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật địa chỉ: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
