using TutorDrive.Dtos.ExamResult;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IExamResultService
    {
        Task<object> ImportFromExcelAsync(IFormFile file);
        Task<List<ExamResult>> SearchAsync(ExamResultSearchDto dto);
        Task<List<ExamResult>> GetHistoryByAccountId(long accountId);
    }
}
