using TutorDrive.Dtos.ExamDto;

namespace TutorDrive.Services.IService
{
    public interface IExamService
    {
        Task<List<ExamDto>> GetAllAsync();
        Task<ExamDto?> GetByIdAsync(long id);
        Task CreateAsync(CreateExamDto dto);
        Task UpdateAsync(UpdateExamDto dto);
        Task DeleteAsync(long id);
        Task<List<UpcomingExamDto>> GetUpcomingExamsForStudentAsync(long accountId);
    }
}
