using TutorDrive.Entities;
using Microsoft.EntityFrameworkCore;

namespace TutorDrive.Database
{
    public class BeContext : DbContext
    {
    
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<InstructorProfile> Staffs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleUsageHistory> VehicleUsageHistories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public BeContext(DbContextOptions<BeContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
