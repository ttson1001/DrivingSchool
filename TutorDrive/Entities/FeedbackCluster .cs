namespace TutorDrive.Entities
{
    public class FeedbackCluster : IEntity
    {
        public long Id { get; set; }

        public string ClusterName { get; set; } 
        public int Count { get; set; }
        public string ExamplesJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
