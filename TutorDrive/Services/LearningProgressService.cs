using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Dtos.LearningProgress.TutorDrive.Dtos.LearningProgress;
using TutorDrive.Dtos.Student;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly IRepository<LearningProgress> _repository;
        private readonly IRepository<StudentProfile> _studentProfileRepository;
        private readonly IRepository<Staff> _staffRepository;
        private readonly IRepository<Section> _sectionRepository;

        public LearningProgressService(
            IRepository<LearningProgress> repository,
            IRepository<StudentProfile> studentProfileRepository,
            IRepository<Staff> staffRepository,
            IRepository<Section> sectionRepository)
        {
            _repository = repository;
            _studentProfileRepository = studentProfileRepository;
            _staffRepository = staffRepository;
            _sectionRepository = sectionRepository;
        }

        public async Task GenerateProgressForCourseAsync(GenerateProgressDto dto)
        {
            var studentProfile = await _studentProfileRepository.Get()
                .FirstOrDefaultAsync(sp => sp.Id == dto.StudentId);

            if (studentProfile == null)
                throw new Exception("StudentProfile không tìm thấy");

            var staff = await _staffRepository.Get()
                .FirstOrDefaultAsync(s => s.Id == dto.TeacherId);

            if (staff == null)
                throw new Exception("Giáo viên không tồn tại");

            var sections = await _sectionRepository.Get()
                .Where(s => s.CourseId == dto.CourseId)
                .ToListAsync();

            if (!sections.Any())
                throw new Exception("Khóa học chưa có phần học (Section)");

            foreach (var section in sections)
            {
                bool exists = await _repository.Get()
                    .AnyAsync(lp => lp.StudentProfileId == dto.StudentId && lp.SectionId == section.Id);

                if (!exists)
                {
                    var lp = new LearningProgress
                    {
                        StudentProfileId = dto.StudentId,
                        CourseId = dto.CourseId,
                        SectionId = section.Id,
                        StaffId = dto.TeacherId,
                        Comment = "",
                        IsCompleted = false,
                        StartDate = dto.StartDate,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _repository.AddAsync(lp);
                }
            }

            await _repository.SaveChangesAsync();
        }

        public async Task UpdateProgressAsync(LearningProgressUpdateDto dto, long accountId)
        {
            var teacher = await _staffRepository.Get()
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (teacher == null)
                throw new Exception("Tài khoản không thuộc giáo viên hợp lệ");

            var progress = await _repository.Get()
                .FirstOrDefaultAsync(lp => lp.Id == dto.Id);

            if (progress == null)
                throw new Exception("Không tìm thấy tiến độ học cần cập nhật");

            progress.IsCompleted = dto.IsCompleted;
            progress.Comment = dto.Comment ?? progress.Comment;
            progress.LastUpdated = DateTime.UtcNow;
            progress.StaffId = teacher.Id;

            if (dto.StartDate.HasValue)
                progress.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                progress.EndDate = dto.EndDate.Value;

            await _repository.SaveChangesAsync();
        }

        public async Task ChangeStaffForCourseAsync(ChangeStaffDto dto, long accountId)
        {
            var currentTeacher = await _staffRepository.Get()
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (currentTeacher == null)
                throw new Exception("Không tìm thấy giáo viên hiện tại");

            var progresses = await _repository.Get()
                .Where(lp => lp.StudentProfileId == dto.StudentId &&
                             lp.CourseId == dto.CourseId &&
                             lp.IsCompleted == false)
                .ToListAsync();

            if (!progresses.Any())
                throw new Exception("Không có tiến độ học nào khả dụng để đổi giáo viên");

            foreach (var lp in progresses)
            {
                if (lp.StaffId != currentTeacher.Id)
                    throw new Exception($"Bạn không có quyền đổi tiến độ ID {lp.Id}");

                lp.StaffId = dto.NewStaffId;
                lp.LastUpdated = DateTime.UtcNow;
            }
            await _repository.SaveChangesAsync();
        }

        public async Task<LearningProgressDetailDto> GetByIdAsync(long id)
        {
            var progress = await _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.Staff)
                    .ThenInclude(s => s.Account)
                .Include(lp => lp.StudentProfile)
                    .ThenInclude(sp => sp.Account)
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (progress == null)
                throw new Exception("Không tìm thấy tiến độ học");

            return new LearningProgressDetailDto
            {
                Id = progress.Id,

                StudentId = progress.StudentProfileId,
                StudentName = progress.StudentProfile?.Account?.FullName,
                StudentEmail = progress.StudentProfile?.Account?.Email,

                CourseId = progress.CourseId,
                CourseName = progress.Course?.Name,

                SectionId = progress.SectionId,
                SectionTitle = progress.Section?.Title,

                TeacherId = progress.StaffId,
                TeacherName = progress.Staff?.Account?.FullName,
                TeacherEmail = progress.Staff?.Account?.Email,

                IsCompleted = progress.IsCompleted,
                Comment = progress.Comment,

                StartDate = progress.StartDate,
                EndDate = progress.EndDate,
                LastUpdated = progress.LastUpdated
            };
        }

        public async Task<TeacherProgressStatsDto> GetTeacherOverviewAsync(long accountId)
        {
            var teacher = await _staffRepository.Get()
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (teacher == null)
                throw new Exception("Không tìm thấy giáo viên hợp lệ");

            var progresses = await _repository.Get()
                .Include(lp => lp.StudentProfile)
                    .ThenInclude(sp => sp.Account)
                .Where(lp => lp.StaffId == teacher.Id)
                .ToListAsync();

            if (!progresses.Any())
            {
                return new TeacherProgressStatsDto
                {
                    TeacherId = teacher.Id,
                    TeacherName = teacher.Account.FullName
                };
            }

            var groupedStudents = progresses
                .GroupBy(lp => lp.StudentProfile)
                .Select(g => new
                {
                    Student = g.Key,
                    IsCompleted = g.All(lp => lp.IsCompleted)
                })
                .ToList();

            var dto = new TeacherProgressStatsDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.Account.FullName
            };

            foreach (var student in groupedStudents)
            {
                var info = new StudentInfoDto
                {
                    Id = student.Student.Id,
                    FullName = student.Student.Account.FullName,
                    Email = student.Student.Account.Email,
                    Avatar = student.Student.Account.Avalar
                };

                if (student.IsCompleted)
                    dto.CompletedStudents.Add(info);
                else
                    dto.ActiveStudents.Add(info);
            }

            return dto;
        }

        public async Task<List<LearningProgressDetailDto>> GetByTeacherAndStudentAsync(long teacherId, long studentId)
        {
            var list = await _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.Staff).ThenInclude(s => s.Account)
                .Include(lp => lp.StudentProfile).ThenInclude(sp => sp.Account)
                .Where(lp => lp.StaffId == teacherId && lp.StudentProfileId == studentId)
                .OrderBy(lp => lp.StartDate)
                .ToListAsync();

            if (!list.Any())
                throw new Exception("Không tìm thấy tiến độ học của học viên này với giáo viên chỉ định");

            return list.Select(lp => new LearningProgressDetailDto
            {
                Id = lp.Id,

                StudentId = lp.StudentProfileId,
                StudentName = lp.StudentProfile?.Account?.FullName,
                StudentEmail = lp.StudentProfile?.Account?.Email,

                CourseId = lp.CourseId,
                CourseName = lp.Course?.Name,

                SectionId = lp.SectionId,
                SectionTitle = lp.Section?.Title,

                TeacherId = lp.StaffId,
                TeacherName = lp.Staff?.Account?.FullName,
                TeacherEmail = lp.Staff?.Account?.Email,

                IsCompleted = lp.IsCompleted,
                Comment = lp.Comment,

                StartDate = lp.StartDate,
                EndDate = lp.EndDate,
                LastUpdated = lp.LastUpdated
            }).ToList();
        }
    }
}
