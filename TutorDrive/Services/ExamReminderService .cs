using Microsoft.EntityFrameworkCore;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;
using TutorDrive.Services.IService.TutorDrive.Services.IService;

public class ExamReminderService : IExamReminderService
{
    private readonly IRepository<RegistrationExam> _registrationExamRepo;
    private readonly IEmailService _emailService;

    public ExamReminderService(
        IRepository<RegistrationExam> registrationExamRepo,
        IEmailService emailService)
    {
        _registrationExamRepo = registrationExamRepo;
        _emailService = emailService;
    }

    public async Task SendExamRemindersAsync()
    {
        var todayUtc = DateTime.UtcNow.Date;
        var targetDate = todayUtc.AddDays(1);

        var regs = await _registrationExamRepo.Get()
            .Include(r => r.StudentProfile).ThenInclude(sp => sp.Account)
            .Include(r => r.Exam)
            .Include(r => r.Course)
            .Where(r =>
                r.Status == RegistrationStatus.Approved &&
                r.Exam != null &&
                r.Exam.ExamDate.Date == targetDate)
            .ToListAsync();

        foreach (var reg in regs)
        {
            var account = reg.StudentProfile.Account;
            if (string.IsNullOrWhiteSpace(account.Email))
                continue;

            var exam = reg.Exam;
            var course = reg.Course;

            var subject = $"Nhắc lịch thi khóa {course?.Name ?? "học"} - ngày {exam.ExamDate:dd/MM/yyyy}";
            var body = $@"
Chào {account.FullName},

Đây là email nhắc bạn về lịch thi sắp tới:

- Khóa học: {course?.Name}
- Ngày thi: {exam.ExamDate:dd/MM/yyyy}
- Giờ thi: {exam.ExamDate:HH:mm}
- Địa điểm: {exam.Location ?? "Sẽ được thông báo sau"}

Vui lòng chuẩn bị đầy đủ giấy tờ và đến sớm trước giờ thi.

Trân trọng,
TutorDrive
";

            await _emailService.SendEmailAsync(account.Email, subject, body);
        }
    }
}
