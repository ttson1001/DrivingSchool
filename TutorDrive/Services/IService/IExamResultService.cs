using TutorDrive.Dtos.ExamResult;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IExamResultService
    {
        Task<object> ImportFromExcelAsync(IFormFile file);
        Task<List<ExamResultDto>> SearchAsync(ExamResultSearchDto dto);
        Task<List<ExamResultDto>> GetHistoryByAccountId(long accountId);
    }
}
