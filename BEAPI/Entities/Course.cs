using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Entities
{
    public class Course : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int? DurationDays { get; set; }
        public decimal? Price { get; set; }
        public ICollection<Section> Sections { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}
