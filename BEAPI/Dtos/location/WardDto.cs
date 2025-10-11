namespace BEAPI.Dtos.Location
{
    public class WardDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ProvinceCode { get; set; }
    }

    public class WardApiResponseDto
    {
        public bool Success { get; set; }
        public List<WardDto> Data { get; set; }
    }
}
