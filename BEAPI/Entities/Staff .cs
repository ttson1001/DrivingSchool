using Google.Api.Gax;
using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Entities
{
    public class Staff : IEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }

        public string LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
