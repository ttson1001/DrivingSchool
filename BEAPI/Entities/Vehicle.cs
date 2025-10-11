using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class Vehicle : IEntity
    {
        public long Id { get; set; }
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public ICollection<VehicleUsageHistory> UsageHistories { get; set; }
    }
}
