using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Transaction;
using TutorDrive.Services.IService;

namespace TutorDrive.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Admin - Xem toàn bộ lịch sử giao dịch (có lọc và phân trang)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Danh sách giao dịch", typeof(ResponseDto))]
        public async Task<IActionResult> GetAllPaymentHistory([FromQuery] TransactionSearchAdminRequest request)
        {
            var response = new ResponseDto();
            try
            {
                var result = await _transactionService.GetAllPagedAsync(request);
                response.Message = "Lấy toàn bộ lịch sử giao dịch thành công.";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy lịch sử giao dịch: {ex.Message}";
                return BadRequest(response);
            }
        }

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Xem lịch sử giao dịch của người dùng (có lọc và phân trang)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Danh sách giao dịch", typeof(ResponseDto))]
        public async Task<IActionResult> GetPaymentHistoryByUser([FromQuery] TransactionSearchRequest request)
        {
            var response = new ResponseDto();
            try
            {

                var userIdStr = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng trong token" });

                if (!long.TryParse(userIdStr, out var userId))
                    return BadRequest(new { success = false, message = "ID người dùng không hợp lệ" });

                var result = await _transactionService.GetByUserPagedAsync(userId ,request);

                response.Message = "Lấy lịch sử giao dịch thành công.";
                response.Data = result;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Lỗi khi lấy lịch sử giao dịch: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
