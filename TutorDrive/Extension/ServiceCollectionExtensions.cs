using TutorDrive.Repositories;
using TutorDrive.Services;
using TutorDrive.Services.IService;
using TutorDrive.Services.IServices;
using TutorDrive.Services.Service;

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
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDriverLicenseService, DriverLicenseService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
    