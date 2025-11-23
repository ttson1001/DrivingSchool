namespace TutorDrive.Entities
{
    public class RegistrationExamExam: IEntity
    {
        public long Id { get; set; }

        public long RegistrationExamId { get; set; }
        public RegistrationExam RegistrationExam { get; set; }

        public long ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
