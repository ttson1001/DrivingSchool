using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.common;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var url = await _cloudinaryService.UploadFileAsync(file);
                return Ok(new { message = "Upload file thành công!", url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Lỗi khi upload file: {ex.Message}" });
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
           Summary = "Upload hình ảnh lên Cloudinary",
           Description = "Upload file ảnh và trả về URL sau khi tải thành công."
       )]
        [SwaggerResponse(200, "Upload thành công", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(UploadImageResponseExample))]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            var response = new ResponseDto();

            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    response.Message = "Không có file được tải lên";
                    return BadRequest(response);
                }

                var url = await _cloudinaryService.UploadImageAsync(request.File);

                response.Message = "Upload hình ảnh thành công";
                response.Data = new UploadImageResponseDto { Url = url };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi upload hình ảnh: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
