using TutorDrive.Entities.Enum.TutorDrive.Entities.Enum;

namespace TutorDrive.Dtos.Course
{
    public class SectionCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class CourseCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndRegistrationDate { get; set; }
        public string ImageUrl { get; set; }
        public int? DurationDays { get; set; }
        public decimal? Price { get; set; }

        public List<SectionCreateDto> Sections { get; set; } = new();
    }

    public class SectionDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int? DurationDays { get; set; }
        public decimal? Price { get; set; }
        public DateTimeOffset? EndRegistrationDate { get; set; }
        public int StudentCount { get; set; }
        public CourseStatus Status { get; set; }

        public List<SectionDto> Sections { get; set; } = new();
    }
}
