using TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class VehicleUsageHistory : IEntity
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public long AccountId { get; set; }
        public Account Account { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public VehicleUsageStatus Status { get; set; } = VehicleUsageStatus.Pending;
    }
}
