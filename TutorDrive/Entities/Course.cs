using System.ComponentModel.DataAnnotations;
using TutorDrive.Entities.Enum.TutorDrive.Entities.Enum;

namespace TutorDrive.Entities
{
    public class Course : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int? DurationDays { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Active;
        public DateTimeOffset? EndRegistrationDate { get; set; }
        public decimal? Price { get; set; }
        public ICollection<Section> Sections { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}
