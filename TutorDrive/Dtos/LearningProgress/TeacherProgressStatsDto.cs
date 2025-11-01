using TutorDrive.Dtos.Student;

namespace TutorDrive.Dtos.LearningProgress
{
    public class TeacherProgressStatsDto
    {
        public long TeacherId { get; set; }
        public string TeacherName { get; set; }

        public List<StudentInfoDto> ActiveStudents { get; set; } = new();
        public List<StudentInfoDto> CompletedStudents { get; set; } = new();

        public int TotalStudents => ActiveStudents.Count + CompletedStudents.Count;
    }
}
