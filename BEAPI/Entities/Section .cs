using Google.Api.Gax;
using System.ComponentModel.DataAnnotations;

namespace BEAPI.Entities
{
    public class Section : IEntity
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public Course Course { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
