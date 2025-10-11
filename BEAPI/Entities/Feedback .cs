namespace BEAPI.Entities
{
    public class Feedback : IEntity
    {
        public long Id { get; set; }
        public long StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public long? StaffId { get; set; }
        public Staff Staff { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
