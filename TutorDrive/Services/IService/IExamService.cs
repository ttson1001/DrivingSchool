using TutorDrive.Dtos.Exam;

namespace TutorDrive.Services.IService
{
    public interface IExamService
    {
        Task<List<ExamDto>> GetAllAsync();
        Task<ExamDto?> GetByIdAsync(long id);
        Task CreateAsync(CreateExamDto dto);
        Task UpdateAsync(UpdateExamDto dto);
        Task DeleteAsync(long id);
        Task<UpcomingExamDto?> GetUpcomingExamsForStudentAsync(long accountId);
    }
}
