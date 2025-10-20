using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.DriverLicense;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace BEAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverLicenseController : ControllerBase
    {
        private readonly IDriverLicenseService _driverLicenseService;

        public DriverLicenseController(IDriverLicenseService driverLicenseService)
        {
            _driverLicenseService = driverLicenseService;
        }

        [HttpPost("[action]")]
        [SwaggerOperation(
            Summary = "Tạo bằng lái mới",
            Description = "Thêm một loại bằng lái xe mới vào hệ thống (VD: A1, B2, C...)"
        )]
        [SwaggerResponse(200, "Tạo bằng lái thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(CreateDriverLicenseResponseExample))]
        public async Task<IActionResult> Create([FromBody] CreateDriverLicenseDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _driverLicenseService.CreateAsync(dto);
                response.Message = "Tạo bằng lái thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo bằng lái: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("[action]")]
        [SwaggerOperation(
            Summary = "Cập nhật thông tin bằng lái",
            Description = "Chỉnh sửa tên hoặc mô tả của loại bằng lái"
        )]
        [SwaggerResponse(200, "Cập nhật bằng lái thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(UpdateDriverLicenseResponseExample))]
        public async Task<IActionResult> Update([FromBody] UpdateDriverLicenseDto dto)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _driverLicenseService.UpdateAsync(dto);
                response.Message = "Cập nhật bằng lái thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật bằng lái: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(
            Summary = "Tìm kiếm / Lấy danh sách bằng lái",
            Description = "Tìm kiếm các loại bằng lái theo từ khóa, hỗ trợ phân trang"
        )]
        [SwaggerResponse(200, "Lấy danh sách bằng lái thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(SearchDriverLicenseResponseExample))]
        public async Task<IActionResult> Search(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _driverLicenseService.SearchAsync(keyword, page, pageSize);
                response.Message = "Lấy danh sách bằng lái thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách bằng lái: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]/{id}")]
        [SwaggerOperation(
            Summary = "Lấy chi tiết bằng lái",
            Description = "Trả về thông tin chi tiết của 1 loại bằng lái theo ID"
        )]
        [SwaggerResponse(200, "Lấy chi tiết bằng lái thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(GetDriverLicenseByIdResponseExample))]
        public async Task<IActionResult> GetById(long id)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _driverLicenseService.GetByIdAsync(id);
                response.Message = "Lấy chi tiết bằng lái thành công";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy chi tiết bằng lái: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
