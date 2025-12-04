using TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Vehicle
{
    public class VehicleUsageHistoryStatusUpdateDto
    {
        public long Id { get; set; }
        public VehicleUsageStatus Status { get; set; }
    }

}
