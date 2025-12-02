using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Dashboard;
using TutorDrive.Dtos.ExamDto;
using TutorDrive.Dtos.Feedbacks;

namespace TutorDrive.Services.IService
{
    public interface IFeedbackService
    {
        Task CreateAsync(FeedbackCreateDto dto, long accountId);
        Task<FeedbackDto?> GetByIdAsync(long id);
        Task<List<FeedbackDto>> GetAllAsync();
        Task<PagedResult<FeedbackDto>> SearchAsync(string? keyword, int page, int pageSize);
        Task UpdateAsync(long id, FeedbackUpdateDto dto);
        Task<List<TopTeacherDto>> GetTopTeachersAsync(int top = 5);
        Task<List<FeedbackDto>> GetHistoryAsync(long accountId);
        Task<HomepageDto> GetHomepageAsync();
    }
}
