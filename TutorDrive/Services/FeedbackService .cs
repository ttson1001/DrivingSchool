using Microsoft.EntityFrameworkCore;
using System;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
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
        private readonly IRepository<InstructorProfile> _staffRepository;
        private readonly IRepository<StudentProfile> _studentProfileRepository;

        public FeedbackService(IRepository<Feedback> repository, IRepository<InstructorProfile> staffRepository, IRepository<StudentProfile> studentProfileRepository)
        {
            _repository = repository;
            _staffRepository = staffRepository;
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task CreateAsync(FeedbackCreateDto dto, long accountId)
        {
            var studentProfileId = await _studentProfileRepository.Get()
                .Where(x => x.AccountId == accountId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (studentProfileId == 0)
                throw new Exception("Không tìm thấy hồ sơ sinh viên.");

            var entity = new Feedback
            {
                StudentProfileId = studentProfileId,
                InstructorProfileId = dto.InstructorProfileId,
                CourseId = dto.CourseId,
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
                    .ThenInclude(sp => sp.Account)
                .Include(f => f.StudentProfile)
                    .ThenInclude(sp => sp.Address)
                        .ThenInclude(a => a.Ward)
                .Include(f => f.StudentProfile)
                    .ThenInclude(sp => sp.Address)
                        .ThenInclude(a => a.Province)
                .Include(f => f.InstructorProfile).ThenInclude(ip => ip.Account)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,

                    Student = new MeDto
                    {
                        AccountId = f.StudentProfile.Account.Id,
                        Email = f.StudentProfile.Account.Email,
                        FullName = f.StudentProfile.Account.FullName,
                        PhoneNumber = f.StudentProfile.Account.PhoneNumber,
                        Avatar = f.StudentProfile.Account.Avatar,
                        Status = f.StudentProfile.Status,

                        CMND = f.StudentProfile.CMND,
                        DOB = f.StudentProfile.DOB,
                        Address = f.StudentProfile.Address == null ? null : new AddressDto
                        {
                            FullAddress = f.StudentProfile.Address.FullAddress,
                            Street = f.StudentProfile.Address.Street,
                            WardId = f.StudentProfile.Address.WardId,
                            WardName = f.StudentProfile.Address.Ward.Name,
                            ProvinceId = f.StudentProfile.Address.ProvinceId,
                            ProvinceName = f.StudentProfile.Address.Ward.Name,
                        }
                    },

                    Instructor = f.InstructorProfile == null ? null : new MeDto
                    {
                        AccountId = f.InstructorProfile.Account.Id,
                        Email = f.InstructorProfile.Account.Email,
                        FullName = f.InstructorProfile.Account.FullName,
                        PhoneNumber = f.InstructorProfile.Account.PhoneNumber,
                        Avatar = f.InstructorProfile.Account.Avatar,

                        LicenseNumber = f.InstructorProfile.LicenseNumber,
                        ExperienceYears = f.InstructorProfile.ExperienceYears
                    }
                })
                .ToListAsync();
        }

        public async Task<FeedbackDto?> GetByIdAsync(long id)
        {
            var f = await _repository.Get()
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(f => f.InstructorProfile).ThenInclude(ip => ip.Account)
                .Include(f => f.Course)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (f == null)
                throw new Exception("Không tìm thấy phản hồi.");

            return new FeedbackDto
            {
                Id = f.Id,
                CourseId = f.CourseId,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt,

                Student = new MeDto
                {
                    AccountId = f.StudentProfile.Account.Id,
                    Email = f.StudentProfile.Account.Email,
                    FullName = f.StudentProfile.Account.FullName,
                    PhoneNumber = f.StudentProfile.Account.PhoneNumber,
                    Avatar = f.StudentProfile.Account.Avatar,
                    Status = f.StudentProfile.Status,

                    CMND = f.StudentProfile.CMND,
                    DOB = f.StudentProfile.DOB,

                    Address = f.StudentProfile.Address == null ? null : new AddressDto
                    {
                        FullAddress = f.StudentProfile.Address.FullAddress,
                        Street = f.StudentProfile.Address.Street,
                        WardId = f.StudentProfile.Address.WardId,
                        WardName = f.StudentProfile.Address.Ward.Name,

                        ProvinceId = f.StudentProfile.Address.ProvinceId,
                        ProvinceName = f.StudentProfile.Address.Province.Name,

                        AccountId = f.StudentProfile.AccountId
                    }
                },

                Instructor = f.InstructorProfile == null ? null : new MeDto
                {
                    AccountId = f.InstructorProfile.Account.Id,
                    Email = f.InstructorProfile.Account.Email,
                    FullName = f.InstructorProfile.Account.FullName,
                    PhoneNumber = f.InstructorProfile.Account.PhoneNumber,
                    Avatar = f.InstructorProfile.Account.Avatar,

                    LicenseNumber = f.InstructorProfile.LicenseNumber,
                    ExperienceYears = f.InstructorProfile.ExperienceYears
                }
            };
        }

        public async Task<PagedResult<FeedbackDto>> SearchAsync(string? keyword, int page, int pageSize)
        {
            var query = _repository.Get();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(f => f.Comment.Contains(keyword));
            }

            query = query.Include(f => f.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(f => f.InstructorProfile).ThenInclude(ip => ip.Account)
                .Include(f => f.Course);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,

                    Student = new MeDto
                    {
                        AccountId = f.StudentProfile.Account.Id,
                        Email = f.StudentProfile.Account.Email,
                        FullName = f.StudentProfile.Account.FullName,
                        PhoneNumber = f.StudentProfile.Account.PhoneNumber,
                        Avatar = f.StudentProfile.Account.Avatar,
                        Status = f.StudentProfile.Status,

                        CMND = f.StudentProfile.CMND,
                        DOB = f.StudentProfile.DOB,

                        Address = f.StudentProfile.Address == null ? null : new AddressDto
                        {
                            FullAddress = f.StudentProfile.Address.FullAddress,
                            Street = f.StudentProfile.Address.Street,

                            WardId = f.StudentProfile.Address.WardId,
                            WardName = f.StudentProfile.Address.Ward.Name,

                            ProvinceId = f.StudentProfile.Address.ProvinceId,
                            ProvinceName = f.StudentProfile.Address.Province.Name,

                            AccountId = f.StudentProfile.AccountId
                        }
                    },

                    Instructor = f.InstructorProfile == null ? null : new MeDto
                    {
                        AccountId = f.InstructorProfile.Account.Id,
                        Email = f.InstructorProfile.Account.Email,
                        FullName = f.InstructorProfile.Account.FullName,
                        PhoneNumber = f.InstructorProfile.Account.PhoneNumber,
                        Avatar = f.InstructorProfile.Account.Avatar,

                        LicenseNumber = f.InstructorProfile.LicenseNumber,
                        ExperienceYears = f.InstructorProfile.ExperienceYears
                    }
                })
                .ToListAsync();

            return new PagedResult<FeedbackDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task UpdateAsync(long id, FeedbackUpdateDto dto)
        {
            var entity = await _repository.Get().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Không tìm thấy phản hồi.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Điểm đánh giá phải từ 1 đến 5.");

            entity.Rating = dto.Rating;
            entity.Comment = dto.Comment;

            await _repository.SaveChangesAsync();
        }

        public async Task<List<FeedbackDto>> GetHistoryAsync(long accountId)
        {
            var student = await _studentProfileRepository.Get()
                            .Include(x => x.Account)
                            .FirstOrDefaultAsync(x => x.AccountId == accountId);

            var instructor = await _staffRepository.Get()
                                .Include(x => x.Account)
                                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            var query = _repository.Get();

            if (student != null)
            {
                query = query.Where(f => f.StudentProfileId == student.Id);
            }
            else if (instructor != null)
            {
                query = query.Where(f => f.InstructorProfileId == instructor.Id);
            }
            else
            {
                throw new Exception("Không tìm thấy hồ sơ người dùng để lấy lịch sử phản hồi.");
            }

            return await query
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(f => f.InstructorProfile).ThenInclude(ip => ip.Account)
                .Include(f => f.Course)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDto
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,

                    Student = new MeDto
                    {
                        AccountId = f.StudentProfile.Account.Id,
                        Email = f.StudentProfile.Account.Email,
                        FullName = f.StudentProfile.Account.FullName,
                        PhoneNumber = f.StudentProfile.Account.PhoneNumber,
                        Avatar = f.StudentProfile.Account.Avatar,
                        Status = f.StudentProfile.Status,

                        CMND = f.StudentProfile.CMND,
                        DOB = f.StudentProfile.DOB,

                        Address = f.StudentProfile.Address == null ? null : new AddressDto
                        {
                            FullAddress = f.StudentProfile.Address.FullAddress,
                            Street = f.StudentProfile.Address.Street,
                            WardId = f.StudentProfile.Address.WardId,
                            WardName = f.StudentProfile.Address.Ward.Name,
                            ProvinceId = f.StudentProfile.Address.ProvinceId,
                            ProvinceName = f.StudentProfile.Address.Province.Name,
                            AccountId = f.StudentProfile.AccountId
                        }
                    },

                    Instructor = f.InstructorProfile == null ? null : new MeDto
                    {
                        AccountId = f.InstructorProfile.Account.Id,
                        Email = f.InstructorProfile.Account.Email,
                        FullName = f.InstructorProfile.Account.FullName,
                        PhoneNumber = f.InstructorProfile.Account.PhoneNumber,
                        Avatar = f.InstructorProfile.Account.Avatar,
                        LicenseNumber = f.InstructorProfile.LicenseNumber,
                        ExperienceYears = f.InstructorProfile.ExperienceYears
                    }
                })
                .ToListAsync();
        }

        public async Task<List<TopTeacherDto>> GetTopTeachersAsync(int top = 5)
        {
            var query = _repository.Get()
                .Where(f => f.InstructorProfileId != null)
                .GroupBy(f => f.InstructorProfileId)
                .Select(g => new
                {
                    InstructorId = g.Key.Value,
                    AverageRating = g.Average(f => f.Rating),
                    TotalFeedbacks = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.TotalFeedbacks)
                .Take(top);

            var topFeedbacks = await query.ToListAsync();

            var instructorIds = topFeedbacks.Select(x => x.InstructorId).ToList();

            var instructors = await _staffRepository.Get()
                .Include(s => s.Account)
                .Where(s => instructorIds.Contains(s.Id))
                .ToListAsync();

            var result = (from f in topFeedbacks
                          join s in instructors on f.InstructorId equals s.Id
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
