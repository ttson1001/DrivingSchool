namespace TutorDrive.Entities
{
    public class Ward : IEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ProvinceCode { get; set; }
    }
}
