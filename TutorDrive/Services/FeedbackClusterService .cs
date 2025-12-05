using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

public class FeedbackClusterService : IFeedbackClusterService
{
    private readonly IRepository<FeedbackCluster> _repo;
    private readonly GeminiAiService _aiService;
    private readonly IRepository<Feedback> _feedbackRepo;


    public FeedbackClusterService(
        IRepository<FeedbackCluster> repo,
        GeminiAiService aiService,
        IRepository<Feedback> feedbackRepo)
    {
        _repo = repo;
        _aiService = aiService;
        _feedbackRepo = feedbackRepo;
    }

    public async Task<List<FeedbackClusterDto>> ProcessFeedbackAsync()
    {
        var comments = await _feedbackRepo.Get()
            .Where(x => !string.IsNullOrEmpty(x.Comment))
            .Select(x => x.Comment)
            .ToListAsync();

        if (!comments.Any())
            throw new Exception("Không có feedback nào để phân nhóm.");

        var oldClusters = await _repo.Get().ToListAsync();
        if (oldClusters.Any())
        {
            _repo.DeleteRange(oldClusters);
            await _repo.SaveChangesAsync();
        }

        var clusters = await _aiService.ClusterAsync(comments);

        foreach (var cluster in clusters)
        {
            var entity = new FeedbackCluster
            {
                ClusterName = cluster.ClusterName,
                Count = cluster.Count,
                ExamplesJson = JsonConvert.SerializeObject(cluster.Examples),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
        }

        await _repo.SaveChangesAsync();

        return clusters;
    }


    public async Task<List<FeedbackCluster>> GetAllClustersAsync()
    {
        return await _repo.Get()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAllAsync()
    {
        var all = await _repo.Get().ToListAsync();
        _repo.DeleteRange(all);
        await _repo.SaveChangesAsync();
    }
}
