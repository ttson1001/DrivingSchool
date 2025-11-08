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
        private readonly IRepository<Section> _sectionRepo;

        public CourseService(IRepository<Course> courseRepo, IRepository<Section> sectionRepo)
        {
            _courseRepo = courseRepo;
            _sectionRepo = sectionRepo;
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
        public async Task UpdateCourseWithSectionsAsync(CourseDto dto)
        {
            var course = await _courseRepo.Get()
                .Include(c => c.Sections)
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (course == null)
                throw new Exception("Không tìm thấy khóa học.");

            course.Name = dto.Name;
            course.Description = dto.Description;
            course.ImageUrl = dto.ImageUrl;
            course.Price = dto.Price;
            course.DurationDays = dto.DurationDays;

            var existingSections = course.Sections.ToList();

            foreach (var sectionDto in dto.Sections)
            {
                if (sectionDto.Id > 0)
                {
                    var existing = existingSections.FirstOrDefault(s => s.Id == sectionDto.Id);
                    if (existing != null)
                    {
                        existing.Title = sectionDto.Title;
                        existing.Description = sectionDto.Description;
                    }
                }
                else
                {
                    course.Sections.Add(new Section
                    {
                        Title = sectionDto.Title,
                        Description = sectionDto.Description
                    });
                }
            }

            var dtoSectionIds = dto.Sections.Select(s => s.Id).ToList();
            var sectionsToRemove = existingSections
                .Where(s => !dtoSectionIds.Contains(s.Id))
                .ToList();

            _sectionRepo.DeleteRange(sectionsToRemove);
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