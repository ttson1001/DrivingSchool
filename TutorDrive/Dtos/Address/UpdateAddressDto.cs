namespace TutorDrive.Dtos.Address
{
    public class UpdateAddressDto
    {
        public long Id { get; set; }
        public string? FullAddress { get; set; }
        public string? Street { get; set; }
        public string ProvinceCode { get; set; }
        public string WardCode { get; set; }

    }
}
