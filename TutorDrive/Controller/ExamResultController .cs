using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.ExamResult;
using TutorDrive.Services.IService;

[ApiController]
[Route("api/[controller]")]
public class ExamResultController : ControllerBase
{
    private readonly IExamResultService _service;

    public ExamResultController(IExamResultService service)
    {
        _service = service;
    }

    [HttpPost("[action]")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Import kết quả thi từ Excel (.xlsx)")]
    public async Task<IActionResult> Import([FromForm] ExamImportRequest request)
    {
        var response = new ResponseDto();

        try
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new ResponseDto { Message = "File không hợp lệ" });

            await _service.ImportFromExcelAsync(request.File);

            response.Message = "Import kết quả thi thành công.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Message = "Lỗi khi import: " + ex.Message;
            return BadRequest(response);
        }
    }
}
