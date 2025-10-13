using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Course;

namespace TutorDrive.Services.IService
{
    public interface ICourseService
    {
        Task CreateCourseWithSectionsAsync(CourseCreateDto dto);
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<PagedResult<CourseDto>> SearchCoursesAsync(string? keyword, int page, int pageSize);
    }
}
