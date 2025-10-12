namespace BEAPI.Dtos.Vehicle
{
    public class VehicleUsageHistoryDto
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public long AccountId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class VehicleUsageHistoryCreateDto
    {
        public long VehicleId { get; set; }
        public long AccountId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class VehicleUsageHistoryUpdateDto
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
