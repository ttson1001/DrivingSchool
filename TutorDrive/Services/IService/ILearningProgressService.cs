namespace TutorDrive.Services.IService
{
    public interface ILearningProgressService
    {
        Task GenerateProgressForCourseAsync(long studentProfileId, long courseId);
    }
}
