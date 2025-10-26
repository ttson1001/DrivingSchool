using Swashbuckle.AspNetCore.Filters;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Account;
using TutorDrive.Dtos.Address;
using TutorDrive.Dtos.common;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Course;
using TutorDrive.Dtos.ExamDto;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Dtos.location;
using TutorDrive.Dtos.Location;
using TutorDrive.Dtos.Registration;
using TutorDrive.Dtos.Role;
using TutorDrive.Dtos.Vehicle;
using TutorDrive.Entities.Enum;

namespace TutorDrive.Extension.SwagerUi
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

    public class GetAllRolesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách vai trò thành công.",
                Data = new List<RoleDto>
                {
                    new RoleDto { Id = 1, Name = "Admin" },
                    new RoleDto { Id = 2, Name = "Tutor" },
                    new RoleDto { Id = 3, Name = "Student" }
                }
            };
        }
    }

    public class SearchAccountsResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() =>
            new ResponseDto
            {
                Message = "Lấy danh sách tài khoản thành công",
                Data = new PagedResult<AccountDto>
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 25,
                    Items = new List<AccountDto>
                    {
                        new AccountDto { Id = 1, Email = "user1@gmail.com", FullName = "Nguyễn Văn A", RoleName = "Student" },
                        new AccountDto { Id = 2, Email = "teacher1@gmail.com", FullName = "Trần Thị B", RoleName = "Tutor" }
                    }
                }
            };
    }

    public class GetAccountByIdResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() =>
            new ResponseDto
            {
                Message = "Lấy chi tiết tài khoản thành công",
                Data = new AccountDto
                {
                    Id = 1,
                    Email = "user1@gmail.com",
                    FullName = "Nguyễn Văn A",
                    RoleName = "Student"
                }
            };
    }

    public class UpdateAccountResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() =>
            new ResponseDto
            {
                Message = "Cập nhật tài khoản thành công",
                Data = new AccountDto
                {
                    Id = 1,
                    Email = "user1@gmail.com",
                    FullName = "Nguyễn Văn A (đã cập nhật)",
                    RoleName = "Student"
                }
            };
    }

    public class CreateFeedbackResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() => new ResponseDto
        {
            Message = "Tạo phản hồi thành công"
        };
    }

    public class GetAllFeedbacksResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() => new ResponseDto
        {
            Message = "Lấy danh sách phản hồi thành công",
            Data = new List<FeedbackDto>
                {
                    new FeedbackDto { Id = 1, StudentProfileId = 2, StaffId = 5, Rating = 5, Comment = "Rất hài lòng", CreatedAt = DateTime.UtcNow },
                    new FeedbackDto { Id = 2, StudentProfileId = 3, StaffId = 4, Rating = 4, Comment = "Ổn nhưng cần cải thiện", CreatedAt = DateTime.UtcNow }
                }
        };
    }

    public class GetFeedbackByIdResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() => new ResponseDto
        {
            Message = "Lấy phản hồi thành công",
            Data = new FeedbackDto
            {
                Id = 1,
                StudentProfileId = 2,
                StaffId = 5,
                Rating = 5,
                Comment = "Tutor rất nhiệt tình",
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    public class SearchFeedbacksResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() => new ResponseDto
        {
            Message = "Lấy danh sách phản hồi thành công (phân trang)",
            Data = new
            {
                Items = new List<FeedbackDto>
                    {
                        new FeedbackDto { Id = 1, Comment = "Tốt", Rating = 5 },
                        new FeedbackDto { Id = 2, Comment = "Ổn", Rating = 4 }
                    },
                TotalItems = 2,
                Page = 1,
                PageSize = 10
            }
        };
    }

    public class UpdateFeedbackResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples() => new ResponseDto
        {
            Message = "Cập nhật phản hồi thành công"
        };
    }

    public class SearchVehiclesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            var pagedResult = new PagedResult<VehicleDto>
            {
                TotalItems = 25,
                Page = 1,
                PageSize = 10,
                Items = new List<VehicleDto>
                {
                    new VehicleDto
                    {
                        Id = 1,
                        PlateNumber = "51F-12345",
                        Brand = "Toyota",
                        Model = "Vios",
                        Status = "Đang hoạt động"
                    },
                    new VehicleDto
                    {
                        Id = 2,
                        PlateNumber = "29A-67890",
                        Brand = "Honda",
                        Model = "Civic",
                        Status = "Đang bảo dưỡng"
                    },
                    new VehicleDto
                    {
                        Id = 3,
                        PlateNumber = "30G-22222",
                        Brand = "Mazda",
                        Model = "CX-5",
                        Status = "Sẵn sàng cho thuê"
                    }
                }
            };

            return new ResponseDto
            {
                Message = "Lấy danh sách xe thành công",
                Data = pagedResult
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

    public class SearchCoursesResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách khóa học thành công",
                Data = new PagedResult<CourseDto>
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 2,
                    Items = new List<CourseDto>
                {
                    new CourseDto
                    {
                        Id = 1,
                        Name = "Lập trình C# cơ bản",
                        Description = "Học các khái niệm lập trình hướng đối tượng với C#",
                        ImageUrl = "https://example.com/images/csharp.jpg",
                        DurationDays = 30,
                        Price = 1200000,
                        Sections = new List<SectionDto>
                        {
                            new SectionDto { Id = 1, Title = "Giới thiệu C#", Description = "Tổng quan về ngôn ngữ C#" },
                            new SectionDto { Id = 2, Title = "OOP trong C#", Description = "Các nguyên lý OOP" }
                        }
                    },
                    new CourseDto
                    {
                        Id = 2,
                        Name = "Spring Boot nâng cao",
                        Description = "Phát triển API RESTful với Spring Boot",
                        ImageUrl = "https://example.com/images/spring.jpg",
                        DurationDays = 45,
                        Price = 1800000,
                        Sections = new List<SectionDto>
                        {
                            new SectionDto { Id = 1, Title = "Cấu trúc dự án", Description = "Tìm hiểu cấu trúc chuẩn của dự án Spring Boot" },
                            new SectionDto { Id = 2, Title = "Bảo mật API", Description = "JWT, Spring Security" }
                        }
                    }
                }
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

    public class UploadImageResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Upload hình ảnh thành công",
                Data = new UploadImageResponseDto
                {
                    Url = "https://res.cloudinary.com/drectazvt/image/upload/v123456789/sample.jpg"
                }
            };
        }
    }

    public class AddressResponseExample : IExamplesProvider<UpdateAddressDto>
    {
        public UpdateAddressDto GetExamples()
        {
            return new UpdateAddressDto
            {
                Id = 1,
                FullAddress = "123 Nguyễn Trãi, Quận 1, TP.HCM",
                Street = "Nguyễn Trãi",
                WardId = 101,
                ProvinceId = 79
            };
        }
    }

    public class ExamResponseExample : IExamplesProvider<ExamDto>
    {
        public ExamDto GetExamples()
        {
            return new ExamDto
            {
                Id = 1,
                CourseId = 101,
                CourseName = "Lập trình Java nâng cao",
                Date = new DateTime(2025, 12, 10),
                Type = "Final Exam",
                Location = "Phòng 204 - Cơ sở 2"
            };
        }
    }

    public class CreateDriverLicenseResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Tạo bằng lái thành công",
                Data = new
                {
                    Id = 1,
                    Name = "B2",
                    Description = "Ô tô dưới 9 chỗ, tải < 3.5 tấn"
                }
            };
        }
    }

    public class RegistrationListResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách đăng ký thành công",
                Data = new
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 2,
                    Items = new[]
                    {
                        new RegistrationListItemDto
                        {
                            Id = 1,
                            StudentId = 101,
                            StudentName = "Nguyen Van A",
                            StudentEmail = "nguyenvana@example.com",
                            CourseId = 10,
                            CourseName = "Spring Boot Masterclass",
                            Status = RegistrationStatus.Approved,
                            Note = "Approved by admin",
                            RegisterDate = DateTime.UtcNow,
                            FileUrls = new List<string>
                            {
                                "https://example.com/file1.pdf",
                                "https://example.com/file2.pdf"
                            }
                        },
                        new RegistrationListItemDto
                        {
                            Id = 2,
                            StudentId = 102,
                            StudentName = "Tran Thi B",
                            StudentEmail = "tranthib@example.com",
                            CourseId = 11,
                            CourseName = "Angular từ cơ bản đến nâng cao",
                            Status = RegistrationStatus.Pending,
                            Note = null,
                            RegisterDate = DateTime.UtcNow,
                            FileUrls = new List<string>()
                        }
                    }
                }
            };
        }
    }

        public class UpdateDriverLicenseResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Cập nhật bằng lái thành công",
                Data = new
                {
                    Id = 1,
                    Name = "C",
                    Description = "Xe tải trên 3.5 tấn"
                }
            };
        }
    }

    public class SearchDriverLicenseResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy danh sách bằng lái thành công",
                Data = new
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 2,
                    Items = new[]
                    {
                            new { Id = 1, Name = "A1", Description = "Xe máy dưới 175cc" },
                            new { Id = 2, Name = "B2", Description = "Ô tô dưới 9 chỗ" }
                        }
                }
            };
        }
    }

    public class GetDriverLicenseByIdResponseExample : IExamplesProvider<ResponseDto>
    {
        public ResponseDto GetExamples()
        {
            return new ResponseDto
            {
                Message = "Lấy chi tiết bằng lái thành công",
                Data = new { Id = 1, Name = "A1", Description = "Xe máy dưới 175cc" }
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
