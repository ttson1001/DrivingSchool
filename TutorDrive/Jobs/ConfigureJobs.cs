using Hangfire;
using TutorDrive.Services.IService;

namespace TutorDrive.Jobs
{
    public static class HangfireJobsConfig
    {
        public static void ConfigureJobs()
        {
            RecurringJob.AddOrUpdate<ILearningProgressReminderService>(
                "daily-learning-reminder",
                job => job.SendReminderAsync(),
                "0 7 * * *"
            );

            RecurringJob.AddOrUpdate<IExamReminderService>(
              "daily-exam-reminder",
              job => job.SendExamRemindersAsync(),
              "0 7 * * *"
          );
        }
    }

}
