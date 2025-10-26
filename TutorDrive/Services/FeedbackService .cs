using Microsoft.EntityFrameworkCore;
using System;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.ExamDto;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _repository;
        private readonly IRepository<Staff> _staffRepository;

        public FeedbackService(IRepository<Feedback> repository, IRepository<Staff> staffRepository)
        {
            _repository = repository;
            _staffRepository = staffRepository;
        }

        public async Task CreateAsync(FeedbackCreateDto dto)
        {
            var entity = new Feedback
            {
                StudentProfileId = dto.StudentProfileId,
                StaffId = dto.StaffId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<FeedbackDto>> GetAllAsync()
        {
            return await _repository.Get()
                .Include(f => f.StudentProfile)
                .Include(f => f.Staff)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    StudentProfileId = f.StudentProfileId,
                    StaffId = f.StaffId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<FeedbackDto?> GetByIdAsync(long id)
        {
            var f = await _repository.Get().FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) throw new Exception("Không tìm thấy phản hồi.");

            return new FeedbackDto
            {
                Id = f.Id,
                StudentProfileId = f.StudentProfileId,
                StaffId = f.StaffId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            };
        }

        public async Task<PagedResult<FeedbackDto>> SearchAsync(string? keyword, int page, int pageSize)
        {
            var query = _repository.Get();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(f => f.Comment.Contains(keyword));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    StudentProfileId = f.StudentProfileId,
                    StaffId = f.StaffId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            var result = new PagedResult<FeedbackDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return result;
        }

        public async Task UpdateAsync(long id, FeedbackUpdateDto dto)
        {
            var entity = await _repository.Get().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new Exception("Không tìm thấy phản hồi.");

            entity.Rating = dto.Rating;
            entity.Comment = dto.Comment;

            await _repository.SaveChangesAsync();
        }

        public async Task<List<TopTeacherDto>> GetTopTeachersAsync(int top = 5)
        {
            var query = _repository.Get()
                .Where(f => f.StaffId != null)
                .GroupBy(f => f.StaffId)
                .Select(g => new
                {
                    StaffId = g.Key.Value,
                    AverageRating = g.Average(f => f.Rating),
                    TotalFeedbacks = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.TotalFeedbacks)
                .Take(top);

            var topFeedbacks = await query.ToListAsync();

            var staffIds = topFeedbacks.Select(x => x.StaffId).ToList();

            var staffs = await _staffRepository.Get()
                .Include(s => s.Account)
                .Where(s => staffIds.Contains(s.Id))
                .ToListAsync();

            var result = (from f in topFeedbacks
                          join s in staffs on f.StaffId equals s.Id
                          select new TopTeacherDto
                          {
                              StaffId = s.Id,
                              TeacherName = s.Account.FullName,
                              Email = s.Account.Email,
                              AverageRating = Math.Round(f.AverageRating, 2),
                              TotalFeedbacks = f.TotalFeedbacks
                          })
                          .OrderByDescending(x => x.AverageRating)
                          .ToList();

            return result;
        }
    }
}
