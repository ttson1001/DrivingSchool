namespace TutorDrive.Dtos.Jobs
{
    public class LessonReminderDto
    {
        public string StudentEmail { get; set; }
        public string StudentName { get; set; }

        public string InstructorName { get; set; }

        public string CourseName { get; set; }

        public string LessonTime { get; set; }
    }

}
