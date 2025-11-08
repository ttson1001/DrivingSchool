namespace TutorDrive.Entities
{
    public class SystemConfig : IEntity
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string? Description { get; set; }
    }
}

