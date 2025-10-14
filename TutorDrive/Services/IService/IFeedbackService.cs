using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Feedbacks;

namespace TutorDrive.Services.IService
{
    public interface IFeedbackService
    {
        Task CreateAsync(FeedbackCreateDto dto);
        Task<FeedbackDto?> GetByIdAsync(long id);
        Task<List<FeedbackDto>> GetAllAsync();
        Task<PagedResult<FeedbackDto>> SearchAsync(string? keyword, int page, int pageSize);
        Task UpdateAsync(long id, FeedbackUpdateDto dto);
    }
}
