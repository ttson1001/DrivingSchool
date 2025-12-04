using TutorDrive.Dtos.Complaint;

namespace TutorDrive.Services.IService
{
    public interface IComplaintService
    {
        Task CreateAsync(long accountId, ComplaintCreateDto dto);
        Task<List<ComplaintDto>> GetAllAsync();
        Task<List<ComplaintDto>> GetMyHistoryAsync(long accountId);
        Task DeleteAsync(long accountId, long id);
        Task ReplyAsync(ComplaintReplyDto dto);
    }
}
