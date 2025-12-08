using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class LearningProgressReminderService : ILearningProgressReminderService
    {
        private readonly ILearningProgressService _learningService;
        private readonly IEmailService _emailService;

        public LearningProgressReminderService(
            ILearningProgressService learningService,
            IEmailService emailService)
        {
            _learningService = learningService;
            _emailService = emailService;
        }

        public async Task SendReminderAsync()
        {
            var tomorrow = DateTime.Now.Date.AddDays(1);

            var lessons = await _learningService.GetLessonsByDateAsync(tomorrow);

            foreach (var lesson in lessons)
            {
                string email = lesson.StudentEmail;
                string subject = "Nhắc lịch học ngày mai";
                string body = $@"
                    Chào {lesson.StudentName},<br><br>
                    Bạn có buổi học vào ngày mai:<br>
                    - Khóa học: {lesson.CourseName}<br>
                    - Thời gian: {lesson.LessonTime}<br>
                    - Giáo viên: {lesson.InstructorName}<br><br>
                    TutorDrive Team.
                    ";

                await _emailService.SendEmailAsync(email, subject, body);
            }
        }
    }

}
