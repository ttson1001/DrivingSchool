namespace TutorDrive.Services.IService
{
    public interface ISystemConfigService
    {
        Task<string?> GetValueAsync(string key);
    }
}
