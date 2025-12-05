using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IFeedbackClusterService
    { 
        Task<List<FeedbackClusterDto>> ProcessFeedbackAsync();

        Task<List<FeedbackCluster>> GetAllClustersAsync();

        Task DeleteAllAsync();
    }
}
