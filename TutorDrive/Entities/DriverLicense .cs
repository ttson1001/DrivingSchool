namespace TutorDrive.Entities
{
    public class DriverLicense : IEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
