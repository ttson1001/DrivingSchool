using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
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

    [HttpGet("[action]")]
    [SwaggerOperation(Summary = "Tìm kiếm kết quả thi theo keyword, ngày, examCode, examId")]
    public async Task<IActionResult> Search([FromQuery] ExamResultSearchDto dto)
    {
        var response = new ResponseDto();

        try
        {
            var results = await _service.SearchAsync(dto);

            response.Data = results;
            response.Message = "Lấy dữ liệu thành công.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDto { Message = "Lỗi khi tìm kiếm: " + ex.Message });
        }
    }

    [HttpGet("[action]")]
    [SwaggerOperation(Summary = "Lấy lịch sử kết quả thi của học sinh từ token")]
    public async Task<IActionResult> GetMyHistory()
    {
        var response = new ResponseDto();

        try
        {
            var accountIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(accountIdStr))
            {
                return Unauthorized(new ResponseDto
                {
                    Message = "Không tìm thấy thông tin người dùng trong token"
                });
            }

            if (!long.TryParse(accountIdStr, out var accountId))
            {
                return BadRequest(new ResponseDto
                {
                    Message = "ID tài khoản không hợp lệ"
                });
            }

            var results = await _service.GetHistoryByAccountId(accountId);

            response.Data = results;
            response.Message = "Lấy lịch sử thi thành công.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDto
            {
                Message = "Lỗi khi lấy lịch sử: " + ex.Message
            });
        }
    }

}
