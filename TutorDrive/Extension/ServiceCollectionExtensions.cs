using BEAPI.PaymentService.VnPay;
using TutorDrive.Repositories;
using TutorDrive.Services;
using TutorDrive.Services.IService;
using TutorDrive.Services.Payment;
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
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IRegistrationFullService, RegistrationFullService>();
            services.AddScoped<ILearningProgressService, LearningProgressService>();
            services.AddScoped<VNPayService>();
            services.AddScoped<ISystemConfigService, SystemConfigService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IRegistrationExamService, RegistrationExamService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamResultService, ExamResultService>();
            services.AddScoped<PayOSService>();
            services.AddScoped<IInstructorDashboardService, InstructorDashboardService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddSingleton<GeminiAiService>();
            services.AddScoped<IFeedbackClusterService, FeedbackClusterService>();
            services.AddScoped<ILearningProgressReminderService, LearningProgressReminderService>();
            services.AddScoped<IExamReminderService, ExamReminderService>();

        }
    }
}
    