using BEAPI.Dtos.Course;
using BEAPI.Entities;
using BEAPI.Repositories;
using BEAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using System;

namespace BEAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course> _courseRepo;

        public CourseService(IRepository<Course> courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task CreateCourseWithSectionsAsync(CourseCreateDto dto)
        {
            var course = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                DurationDays = dto.DurationDays,
                Price = dto.Price,
                Sections = dto.Sections.Select(s => new Section
                {
                    Title = s.Title,
                    Description = s.Description
                }).ToList()
            };

            await _courseRepo.AddAsync(course);
            await _courseRepo.SaveChangesAsync();
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            return await _courseRepo.Get()
                .Include(c => c.Sections)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    DurationDays = c.DurationDays,
                    Price = c.Price,
                    Sections = c.Sections.Select(s => new SectionDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}