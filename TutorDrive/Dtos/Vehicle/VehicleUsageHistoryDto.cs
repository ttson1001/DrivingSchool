using TutorDrive.Dtos.account;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Vehicle
{
    public class VehicleUsageHistoryDto
    {
        public long Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public VehicleUsageStatus Status { get; set; }
        public MeDto Account { get; set; }
        public VehicleDto Vehicle { get; set; }
    }


    public class VehicleUsageHistoryCreateDto
    {
        public long VehicleId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }

    public class VehicleUsageHistoryUpdateDto
    {
        public long Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public VehicleUsageStatus Status { get; set; }
    }
}
