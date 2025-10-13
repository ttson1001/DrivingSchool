using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Entities
{
    public class Address : IEntity
    {
        public long Id { get; set; }
        public string FullAddress { get; set; }
        public string Street { get; set; }
        public long WardId { get; set; }
        public Ward Ward { get; set; }
        public Province Province { get; set; }
        public long ProvinceId { get; set; }
    }
}
