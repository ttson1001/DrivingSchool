using TutorDrive.Repositories;
using TutorDrive.Services;
using TutorDrive.Services.IService;
using TutorDrive.Services.IServices;

namespace TutorDrive.Extension
{ 
    public static class ServiceCollectionExtensions
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProvinceSyncService, ProvinceSyncService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IVehicleUsageHistoryService, VehicleUsageHistoryService>();
        }
    }
}
    