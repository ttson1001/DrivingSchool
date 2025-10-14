using Microsoft.EntityFrameworkCore;
using System;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _repository;

        public FeedbackService(IRepository<Feedback> repository)
        {
            _repository = repository;
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
    }
}
