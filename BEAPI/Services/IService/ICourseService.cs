using BEAPI.Dtos.Course;

namespace BEAPI.Services.IService
{
    public interface ICourseService
    {
        Task CreateCourseWithSectionsAsync(CourseCreateDto dto);
        Task<List<CourseDto>> GetAllCoursesAsync();
    }
}
