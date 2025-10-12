using BEAPI.Dtos.account;
using BEAPI.Dtos.common;
using BEAPI.Dtos.Course;
using BEAPI.Dtos.location;
using BEAPI.Dtos.Location;
using BEAPI.Dtos.Vehicle;
using Swashbuckle.AspNetCore.Filters;

namespace BEAPI.Extension.SwagerUi
{
    public class RegisterResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Đăng kí thành công",
                Data = null
            };
        }
    }

    public class CreateCourseResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Tạo course cùng sections thành công",
                Data = null
            };
        }
    }

    public class LoginResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Đăng nhập thành công",
                Data = new LoginReponseDto
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                }
            };
        }
    }
    public class GetAllCoursesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách khóa học lái xe thành công",
                Data = new List<CourseDto>
                {
                    new CourseDto
                    {
                        Id = 1,
                        Name = "Khóa học lái xe B1 cơ bản",
                        Description = "Học lái xe B1 từ cơ bản đến nâng cao",
                        Price = 2000,
                        ImageUrl = "https://example.com/images/b1-course.jpg",
                        DurationDays = 30,
                        Sections = new List<SectionDto>
                        {
                            new SectionDto { Id = 1, Title = "Giới thiệu về xe và luật giao thông", Description = "Tổng quan về xe và luật giao thông" },
                            new SectionDto { Id = 2, Title = "Thực hành lái xe cơ bản", Description = "Thực hành điều khiển xe trên sân tập" },
                            new SectionDto { Id = 3, Title = "Lái xe trong đô thị", Description = "Lái xe trên đường phố, xử lý tình huống thực tế" }
                        }
                    },
                    new CourseDto
                    {
                        Id = 2,
                        Name = "Khóa học lái xe B2 nâng cao",
                        Description = "Học lái xe B2 nâng cao, chuẩn bị thi sát hạch",
                        Price = 4000,
                        ImageUrl = "https://example.com/images/b2-course.jpg",
                        DurationDays = 45,
                        Sections = new List<SectionDto>
                        {
                            new SectionDto { Id = 1, Title = "Ôn lý thuyết và luật giao thông nâng cao", Description = "Ôn tập kiến thức lý thuyết" },
                            new SectionDto { Id = 2, Title = "Thực hành lái xe nâng cao", Description = "Lái xe trên đường trường, kỹ thuật lùi, ghép xe" },
                            new SectionDto { Id = 3, Title = "Thi sát hạch giả lập", Description = "Thi thử để làm quen với bài thi thực tế" }
                        }
                    }
                }
            };
        }
    }

    public class GetAllProvincesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách tỉnh/thành công",
                Data = new List<ProvinceExternalDto>
                {
                    new ProvinceExternalDto { Code = "01", Name = "Hà Nội", Type = "Thành phố" },
                    new ProvinceExternalDto { Code = "02", Name = "Hồ Chí Minh", Type = "Thành phố" }
                }
            };
        }
    }

    public class GetWardsByProvinceResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách phường/xã thành công",
                Data = new List<WardDto>
                {
                    new WardDto { Code = "001", Name = "Phường 1", Type = "Phường", ProvinceCode = "01" },
                    new WardDto { Code = "002", Name = "Phường 2", Type = "Phường", ProvinceCode = "01" }
                }
            };
        }
    }

    public class SyncResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Đã đồng bộ thành công!",
                Data = null
            };
        }
    }

    public class GetAllVehiclesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách xe thành công",
                Data = new List<VehicleDto>
                {
                    new VehicleDto
                    {
                        Id = 1,
                        PlateNumber = "30A-123.45",
                        Brand = "Toyota",
                        Model = "Corolla",
                        Status = "Active"
                    },
                    new VehicleDto
                    {
                        Id = 2,
                        PlateNumber = "29B-678.90",
                        Brand = "Honda",
                        Model = "Civic",
                        Status = "Inactive"
                    }
                }
            };
        }
    }

    public class VehicleCreateResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Tạo xe thành công",
                Data = null
            };
        }
    }

    public class VehicleUpdateResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Cập nhật xe thành công",
                Data = null
            };
        }
    }

    public class GetAllVehicleUsageHistoryResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách thành công",
                Data = new List<VehicleUsageHistoryDto>
                {
                    new VehicleUsageHistoryDto
                    {
                        Id = 1,
                        VehicleId = 101,
                        AccountId = 201,
                        StartTime = DateTime.Now.AddHours(-2),
                        EndTime = DateTime.Now
                    },
                    new VehicleUsageHistoryDto
                    {
                        Id = 2,
                        VehicleId = 102,
                        AccountId = 202,
                        StartTime = DateTime.Now.AddHours(-5),
                        EndTime = DateTime.Now.AddHours(-1)
                    }
                }
            };
        }
    }

    public class GetVehicleUsageHistoryByIdResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy dữ liệu thành công",
                Data = new VehicleUsageHistoryDto
                {
                    Id = 1,
                    VehicleId = 101,
                    AccountId = 201,
                    StartTime = DateTime.Now.AddHours(-2),
                    EndTime = DateTime.Now
                }
            };
        }
    }

    public class GetVehicleUsageHistoryByVehicleResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách thành công",
                Data = new List<VehicleUsageHistoryDto>
                {
                    new VehicleUsageHistoryDto
                    {
                        Id = 1,
                        VehicleId = 101,
                        AccountId = 201,
                        StartTime = DateTime.Now.AddHours(-2),
                        EndTime = DateTime.Now
                    },
                    new VehicleUsageHistoryDto
                    {
                        Id = 3,
                        VehicleId = 101,
                        AccountId = 203,
                        StartTime = DateTime.Now.AddHours(-6),
                        EndTime = DateTime.Now.AddHours(-3)
                    }
                }
            };
        }
    }
    public class GetVehicleUsageHistoryByAccountResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách thành công",
                Data = new List<VehicleUsageHistoryDto>
                {
                    new VehicleUsageHistoryDto
                    {
                        Id = 2,
                        VehicleId = 102,
                        AccountId = 201,
                        StartTime = DateTime.Now.AddHours(-5),
                        EndTime = DateTime.Now.AddHours(-1)
                    },
                    new VehicleUsageHistoryDto
                    {
                        Id = 4,
                        VehicleId = 103,
                        AccountId = 201,
                        StartTime = DateTime.Now.AddHours(-10),
                        EndTime = DateTime.Now.AddHours(-7)
                    }
                }
            };
        }
    }

    public class CreateVehicleUsageHistoryResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Tạo lịch sử thành công",
                Data = null
            };
        }
    }

    public class UpdateVehicleUsageHistoryResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Cập nhật thành công",
                Data = null
            };
        }
    }
}
