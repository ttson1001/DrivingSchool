﻿using System.ComponentModel.DataAnnotations;

namespace TutorDrive.Entities
{
    public class StudentProfile : IEntity
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public string? CMND { get; set; }
        public DateTime? DOB { get; set; }
        public long? AddressId { get; set; }
        public Address Address { get; set; }
        public string Status { get; set; }
        public IList<Registration> Registrations { get; set; }
        public IList<ExamResult> ExamResults { get; set; }
        public IList<Feedback> Feedbacks { get; set; }
        public LearningProgress LearningProgress { get; set; }
    }
}
