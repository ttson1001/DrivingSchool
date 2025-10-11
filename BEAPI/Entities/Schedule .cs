using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class Schedule : IEntity
    {
        public long Id { get; set; }
        public long? SectionId { get; set; }
        public Section Section { get; set; }
        public long? StaffId { get; set; }
        public Staff Staff { get; set; }
        public DateTime ScheduledAt { get; set; }
        public bool CheckedIn { get; set; }
    }
}
