using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.Registration;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class RegistrationFullService : IRegistrationFullService
    {
        private readonly IRepository<Registration> _repositoryRegistration;
        private readonly IRepository<StudentProfile> _repositoryStudentProfile;
        private readonly IRepository<Account> _repositoryAccount;

        public RegistrationFullService(IRepository<StudentProfile> repositoryStudentProfile, IRepository<Account> repositoryAccount, IRepository<Registration> repositoryRegistration)
        {
            _repositoryStudentProfile = repositoryStudentProfile;
            _repositoryAccount = repositoryAccount;
            _repositoryRegistration = repositoryRegistration;
        }

        public async Task RegisterFullAsync(long accountId, RegistrationFullCreateDto dto)
        {
            var profile = await _repositoryStudentProfile.Get()
                .Include(s => s.Registrations)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (profile == null)
            {
                throw new InvalidOperationException("Học viên chưa có hồ sơ cá nhân. Vui lòng hoàn tất hồ sơ trước khi đăng ký.");
            }

            StudyDay studyDays = StudyDay.None;
            foreach (var day in dto.StudyDays)
            {
                if (Enum.TryParse<StudyDay>(day, true, out var parsedDay))
                    studyDays |= parsedDay;
            }

            var registration = new Registration
            {
                StudentProfileId = profile.Id,
                CourseId = dto.CourseId,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Note = dto.Note,
                Status = Entities.Enum.RegistrationStatus.Pending,
                RegisterDate = DateTime.UtcNow,

                StartDateTime = dto.StartDateTime,
                StudyDays = studyDays,

                Files = new List<RegistrationFile>()
            };

            if (!string.IsNullOrEmpty(dto.CCCDFront))
                registration.Files.Add(new RegistrationFile
                {
                    Url = dto.CCCDFront,
                    FileType = Entities.Enum.FileType.CCCD_Front
                });

            if (!string.IsNullOrEmpty(dto.CCCDBack))
                registration.Files.Add(new RegistrationFile
                {
                    Url = dto.CCCDBack,
                    FileType = Entities.Enum.FileType.CCCD_Back
                });

            await _repositoryRegistration.AddAsync(registration);
            await _repositoryRegistration.SaveChangesAsync();
        }
        public async Task<PagedResult<RegistrationListItemDto>> SearchAsync(RegistrationSearchDto filter)
        {
            var query = _repositoryRegistration.Get()
                .Include(r => r.StudentProfile).ThenInclude(s => s.Account)
                .Include(r => r.Course)
                .Include(r => r.Files)
                .AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.RegisterDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(r => r.RegisterDate <= filter.ToDate.Value);

            var totalItems = await query.CountAsync();

            var items = query
            .OrderByDescending(r => r.RegisterDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsEnumerable()
            .Select(r => new RegistrationListItemDto
            {
                Id = r.Id,
                StudentId = r.StudentProfileId,
                StudentName = r.StudentProfile.Account.FullName,
                StudentEmail = r.StudentProfile.Account.Email,
                CourseId = r.CourseId,
                CourseName = r.Course.Name,
                FullName = r.FullName,
                CCCD = r.StudentProfile.CMND,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                Status = r.Status,
                Note = r.Note,
                RegisterDate = r.RegisterDate,
                FileUrls = r.Files.Select(f => f.Url).ToList(),
                StartDateTime = r.StartDateTime,

                StudyDays = string.Join(", ", Enum.GetValues(typeof(StudyDay))
                    .Cast<StudyDay>()
                    .Where(d => d != StudyDay.None && r.StudyDays.HasFlag(d))
                    .Select(d => d.ToString()))
            })
            .ToList();

            return new PagedResult<RegistrationListItemDto>
            {
                TotalItems = totalItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Items = items
            };
        }

        public async Task UpdateStatusAsync(UpdateRegistrationStatusDto dto)
        {
            var registration = await _repositoryRegistration.Get()
                .FirstOrDefaultAsync(r => r.Id == dto.RegistrationId);

            if (registration == null)
                throw new Exception("Không tìm thấy đơn đăng ký.");

            registration.Status = dto.Status;
            if (!string.IsNullOrEmpty(dto.Note))
                registration.Note = dto.Note;

            _repositoryRegistration.Update(registration);
            await _repositoryRegistration.SaveChangesAsync();

        }


        public async Task<PagedResult<RegistrationListItemDto>> GetByAccountIdAsync(long accountId, RegistrationSearchDto filter)
        {
            var studentProfile = await _repositoryStudentProfile.Get()
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);

            if (studentProfile == null)
                throw new Exception("Không tìm thấy hồ sơ học viên cho tài khoản này.");

            var query = _repositoryRegistration.Get()
                .Include(r => r.StudentProfile).ThenInclude(s => s.Account)
                .Include(r => r.Course)
                .Include(r => r.Files)
                .Where(r => r.StudentProfileId == studentProfile.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(r =>
                    r.Course.Name.Contains(filter.Keyword) ||
                    (r.Note != null && r.Note.Contains(filter.Keyword)));
            }

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(r => r.RegisterDate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(r => r.RegisterDate <= filter.ToDate.Value);

            var totalItems = await query.CountAsync();

            var items = query
            .OrderByDescending(r => r.RegisterDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsEnumerable()
            .Select(r => new RegistrationListItemDto
            {
                Id = r.Id,
                StudentId = r.StudentProfileId,
                StudentName = r.StudentProfile.Account.FullName,
                StudentEmail = r.StudentProfile.Account.Email,
                CourseId = r.CourseId,
                CourseName = r.Course.Name,
                FullName = r.FullName,
                CCCD = r.StudentProfile.CMND,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                Status = r.Status,
                Note = r.Note,
                RegisterDate = r.RegisterDate,
                FileUrls = r.Files.Select(f => f.Url).ToList(),
                StartDateTime = r.StartDateTime,

                StudyDays = string.Join(", ", Enum.GetValues(typeof(StudyDay))
                    .Cast<StudyDay>()
                    .Where(d => d != StudyDay.None && r.StudyDays.HasFlag(d))
                    .Select(d => d.ToString()))
            })
            .ToList();

            return new PagedResult<RegistrationListItemDto>
            {
                TotalItems = totalItems,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Items = items
            };
        }
    }
}
