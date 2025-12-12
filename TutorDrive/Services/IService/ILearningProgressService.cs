using TutorDrive.Dtos.Jobs;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Dtos.LearningProgress.TutorDrive.Dtos.LearningProgress;

namespace TutorDrive.Services.IService
{
    public interface ILearningProgressService
    {
        Task GenerateProgressForCourseAsync(GenerateProgressDto dto);
        Task UpdateProgressAsync(LearningProgressUpdateDto dto, long teacherId);
        Task ChangeStaffForCourseAsync(ChangeStaffDto dto);
        Task<LearningProgressDetailDto> GetByIdAsync(long id);
        Task<TeacherProgressStatsDto> GetTeacherOverviewAsync(long accountId);
        Task<List<LearningProgressDetailDto>> GetByTeacherAndStudentAsync(long teacherId, long studentId);
        Task<List<CourseLearningProgressGroupDto>> GetByStudentGroupedAsync(long accountId, bool? isCompleted = null);
        Task<List<CourseProgressGroupDto>> GetByInstructorGroupedAsync(long instructorAccountId, bool? isCompleted = null);
        Task<List<CourseProgressGroupDto>> GetAdminLearningProgressAsync(bool? isCompleted = null);
        Task UpdateStudentTrainingAsync(long studentId, UpdateStudentTrainingDto dto, long teacherAccountId);
        Task<List<CompletedCourseDto>> GetCompletedCoursesByStudentAsync(long accountId);
        Task<List<LessonReminderDto>> GetLessonsByDateAsync(DateTime date);

    }
}
