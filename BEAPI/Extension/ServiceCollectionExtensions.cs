using BEAPI.Repositories;
using BEAPI.Services;
using BEAPI.Services.IService;
using BEAPI.Services.IServices;

namespace BEAPI.Extension
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
    