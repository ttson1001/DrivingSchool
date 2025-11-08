namespace TutorDrive.Entities
{
    public class InstructorProfile: IEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }

        public string LicenseNumber { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
