namespace TutorDrive.Dtos.Vehicle
    {
        public class VehicleDto
        {
            public long Id { get; set; }
            public string PlateNumber { get; set; }
            public string ImageUrl { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
        }

        public class VehicleCreateDto
        {
            public string PlateNumber { get; set; }
            public string ImageUrl { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
        }

        public class VehicleUpdateDto
        {
            public long Id { get; set; }
            public string ImageUrl { get; set; }
            public string Brand { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
        }
    }
