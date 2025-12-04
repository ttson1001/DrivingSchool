using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Course;
using TutorDrive.Entities.Enum.TutorDrive.Entities.Enum;

namespace TutorDrive.Services.IService
{
    public interface ICourseService
    {
        Task CreateCourseWithSectionsAsync(CourseCreateDto dto);
        Task UpdateCourseWithSectionsAsync(CourseDto dto);
        Task SoftDeleteCourseAsync(long id);
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<PagedResult<CourseDto>> SearchCoursesAsync(string? keyword, int page, int pageSize, CourseStatus? status);
    }
}
