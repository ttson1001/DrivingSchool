using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Course;
using TutorDrive.Dtos.RegistrationExam;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services.Service
{
    public class RegistrationExamService : IRegistrationExamService
    {
        private readonly IRepository<RegistrationExam> _registrationRepo;
        private readonly IRepository<StudentProfile> _studentProfileRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<Exam> _examRepo;

        public RegistrationExamService(
            IRepository<RegistrationExam> registrationRepo,
            IRepository<StudentProfile> studentProfileRepo,
            IRepository<Course> courseRepo,
            IRepository<Exam> examRepo)
        {
            _registrationRepo = registrationRepo;
            _studentProfileRepo = studentProfileRepo;
            _courseRepo = courseRepo;
            _examRepo = examRepo;
        }

        public async Task<RegistrationExamDto> SubmitAsync(RegistrationExamCreateDto dto, long accountId)
        {
            var student = await _studentProfileRepo.Get()
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (student == null)
                throw new Exception("Không tìm thấy hồ sơ học viên.");

            if (dto.CccdFront == null ||
                dto.CccdBack == null ||
                dto.Avatar3x4 == null ||
                dto.HealthCertificate == null ||
                dto.ApplicationForm == null)
            {
                throw new Exception("Thiếu file bắt buộc trong hồ sơ.");
            }

            var course = await _courseRepo.Get().FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null)
                throw new Exception($"CourseId {dto.CourseId} không tồn tại.");

            var entity = new RegistrationExam
            {
                StudentProfileId = student.Id,
                CourseId = dto.CourseId,

                CccdFront = dto.CccdFront,
                CccdBack = dto.CccdBack,
                Avatar3x4 = dto.Avatar3x4,
                HealthCertificate = dto.HealthCertificate,
                ApplicationForm = dto.ApplicationForm
            };

            await _registrationRepo.AddAsync(entity);
            await _registrationRepo.SaveChangesAsync();

            return MapToDto(entity, student);
        }

        public async Task<PagedResult<RegistrationExamDto>> SearchAsync(RegistrationExamSearchRequest request)
        {
            var query = _registrationRepo.Get();

            if (request.Status.HasValue)
                query = query.Where(r => r.Status == request.Status.Value);

            query = query.Include(r => r.StudentProfile).ThenInclude(sp => sp.Account).Include(r => r.Course);
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(r =>
                    r.StudentProfile.Account.FullName.Contains(keyword) ||
                    r.StudentProfile.Account.Email.Contains(keyword) ||
                    r.StudentProfile.Account.PhoneNumber.Contains(keyword));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = items.Select(r => MapToDto(r, r.StudentProfile)).ToList();

            return new PagedResult<RegistrationExamDto>
            {
                Items = result,
                TotalItems = totalItems,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<RegistrationExamDto?> GetByIdAsync(long id)
        {
            var entity = await _registrationRepo.Get()
                .Include(r => r.StudentProfile)
                    .ThenInclude(sp => sp.Account)
                    .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null) return null;

            return MapToDto(entity, entity.StudentProfile);
        }

        public async Task<List<RegistrationExamDto>> GetMyRegistrationsAsync(long accountId)
        {
            var student = await _studentProfileRepo.Get()
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (student == null)
                throw new Exception("Không tìm thấy hồ sơ học viên.");

            var items = await _registrationRepo.Get().Include(r => r.Course)
                .Where(r => r.StudentProfileId == student.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return items.Select(r => MapToDto(r, student)).ToList();
        }

        public async Task<RegistrationExamDto> UpdateByStudentAsync(RegistrationExamUpdateDto dto, long accountId)
        {
            var student = await _studentProfileRepo.Get()
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (student == null)
                throw new Exception("Không tìm thấy hồ sơ học viên.");

            var entity = await _registrationRepo.Get()
                .FirstOrDefaultAsync(r => r.Id == dto.Id && r.StudentProfileId == student.Id);

            if (entity == null)
                throw new Exception("Không tìm thấy hồ sơ đăng ký.");

            if (entity.Status == RegistrationStatus.Approved)
                throw new Exception("Hồ sơ đã được duyệt, không thể chỉnh sửa.");

            if (dto.CccdFront != null)
                entity.CccdFront = dto.CccdFront;

            if (dto.CccdBack != null)
                entity.CccdBack = dto.CccdBack;

            if (dto.Avatar3x4 != null)
                entity.Avatar3x4 = dto.Avatar3x4;

            if (dto.HealthCertificate != null)
                entity.HealthCertificate = dto.HealthCertificate;

            if (dto.ApplicationForm != null)
                entity.ApplicationForm = dto.ApplicationForm;

            entity.UpdatedAt = DateTime.UtcNow;

            await _registrationRepo.SaveChangesAsync();

            return MapToDto(entity, student);
        }

        public async Task UpdateStatusAsync(RegistrationExamStatusUpdateDto dto, long adminAccountId)
        {
            var entity = await _registrationRepo.Get()
                .Include(r => r.StudentProfile)
                .Include(r => r.Course)
                .Include(r => r.Exams)
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (entity == null)
                throw new Exception("Không tìm thấy hồ sơ đăng ký.");

            entity.Status = dto.Status;
            entity.Comment = dto.Comment;
            entity.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == RegistrationStatus.Approved)
            {
                if (entity.CourseId == null)
                    throw new Exception("Hồ sơ chưa chọn khóa học, không thể gán kỳ thi.");

                var exams = await _examRepo.Get()
                    .Where(e => e.CourseId == entity.CourseId)
                    .ToListAsync();

                if (!exams.Any())
                    throw new Exception("Khóa học này chưa có kỳ thi nào.");

                entity.Exams.Clear();

                foreach (var exam in exams)
                {
                    entity.Exams.Add(new RegistrationExamExam
                    {
                        RegistrationExamId = entity.Id,
                        ExamId = exam.Id
                    });
                }
            }

            await _registrationRepo.SaveChangesAsync();
        }


        private RegistrationExamDto MapToDto(RegistrationExam entity, StudentProfile student)
        {
            return new RegistrationExamDto
            {
                Id = entity.Id,
                StudentProfileId = student.Id,
                AccountId = student.AccountId,
                StudentName = student.Account.FullName,
                Email = student.Account.Email,
                PhoneNumber = student.Account.PhoneNumber,

                CccdFront = entity.CccdFront,
                CccdBack = entity.CccdBack,
                Avatar3x4 = entity.Avatar3x4,
                HealthCertificate = entity.HealthCertificate,
                ApplicationForm = entity.ApplicationForm,

                Status = entity.Status,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

                Course = entity.Course == null ? null : new CourseDto
                {
                    Id = entity.Course.Id,
                    Name = entity.Course.Name,
                    Description = entity.Course.Description,
                    ImageUrl = entity.Course.ImageUrl,
                    DurationDays = entity.Course.DurationDays,
                    Price = entity.Course.Price
                }
            };
        }

    }
}
