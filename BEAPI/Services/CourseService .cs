using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Course;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace TutorDrive.Services
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

        public async Task<PagedResult<CourseDto>> SearchCoursesAsync(string? keyword, int page, int pageSize)
        {
            IQueryable<Course> query = _courseRepo.Get().Include(z => z.Sections);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c =>
                    c.Name.Contains(keyword) ||
                    (c.Description != null && c.Description.Contains(keyword)));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new PagedResult<CourseDto>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }
    }
}