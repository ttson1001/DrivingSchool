namespace TutorDrive.Dtos.location
{
    public class ProvinceExternalDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ProvinceApiResponse
    {
        public bool Success { get; set; }
        public List<ProvinceExternalDto> Data { get; set; }
    }

}
