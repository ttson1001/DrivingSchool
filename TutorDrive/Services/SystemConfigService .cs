using Microsoft.EntityFrameworkCore;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly IRepository<SystemConfig> _repository;

        public SystemConfigService(IRepository<SystemConfig> repository)
        {
            _repository = repository;
        }

        public async Task<string?> GetValueAsync(string key)
        {
            var config = await _repository.Get().FirstOrDefaultAsync(x => x.Key == key);
            return config?.Value;
        }
    }

}
