namespace TutorDrive.Services.IService
{
    public interface IExamResultService
    {
        Task<object> ImportFromExcelAsync(IFormFile file);
    }
}
