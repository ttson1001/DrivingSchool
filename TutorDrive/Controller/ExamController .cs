using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.ExamDto;
using TutorDrive.Extension.SwagerUi;
using TutorDrive.Services.IService;

namespace TutorDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả kỳ thi")]
        [SwaggerResponse(StatusCodes.Status200OK, "Danh sách kỳ thi", typeof(ResponseDto))]
        [SwaggerResponseExample(200, typeof(ExamResponseExample))]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDto();
            try
            {
                var exams = await _examService.GetAllAsync();
                response.Message = "Lấy danh sách kỳ thi thành công";
                response.Data = exams;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy danh sách kỳ thi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin kỳ thi theo ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Thông tin kỳ thi", typeof(ResponseDto))]
        public async Task<IActionResult> GetById(long id)
        {
            var response = new ResponseDto();
            try
            {
                var exam = await _examService.GetByIdAsync(id);
                if (exam == null)
                {
                    response.Message = "Không tìm thấy kỳ thi.";
                    return NotFound(response);
                }

                response.Message = "Lấy thông tin kỳ thi thành công";
                response.Data = exam;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy kỳ thi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo mới kỳ thi")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tạo kỳ thi thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _examService.CreateAsync(dto);
                response.Message = "Tạo kỳ thi thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi tạo kỳ thi: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật kỳ thi")]
        [SwaggerResponse(StatusCodes.Status200OK, "Cập nhật kỳ thi thành công", typeof(ResponseDto))]
        public async Task<IActionResult> Update(UpdateExamDto dto)
        {
            var response = new ResponseDto();
            try
            {
                await _examService.UpdateAsync(dto);
                response.Message = "Cập nhật kỳ thi thành công";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi cập nhật kỳ thi: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
