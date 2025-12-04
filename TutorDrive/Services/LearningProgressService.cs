using Microsoft.EntityFrameworkCore;
using PayOS.Exceptions;
using TutorDrive.Dtos.account;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
using TutorDrive.Dtos.Feedbacks;
using TutorDrive.Dtos.LearningProgress;
using TutorDrive.Dtos.LearningProgress.TutorDrive.Dtos.LearningProgress;
using TutorDrive.Dtos.Student;
using TutorDrive.Entities;
using TutorDrive.Entities.Enum;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly IRepository<LearningProgress> _repository;
        private readonly IRepository<StudentProfile> _studentProfileRepository;
        private readonly IRepository<InstructorProfile> _staffRepository;
        private readonly IRepository<Section> _sectionRepository;
        private readonly IRepository<Registration> _registrationRepository;
        private readonly IRepository<Feedback> _feedbackRepository;
        private readonly IRepository<RegistrationExam> _registrationExamRepository;

        public LearningProgressService(
            IRepository<LearningProgress> repository,
            IRepository<StudentProfile> studentProfileRepository,
            IRepository<InstructorProfile> staffRepository,
            IRepository<Section> sectionRepository,
            IRepository<Registration> registrationRepository,
            IRepository<Feedback> feedbackRepository,
            IRepository<RegistrationExam> registrationExamRepository)
        {
            _repository = repository;
            _studentProfileRepository = studentProfileRepository;
            _staffRepository = staffRepository;
            _sectionRepository = sectionRepository;
            _registrationRepository = registrationRepository;
            _feedbackRepository = feedbackRepository;
            _registrationExamRepository = registrationExamRepository;
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

            // Lấy Registration để biết StudyDays
            var registration = await _registrationRepository.Get()
                .FirstOrDefaultAsync(r => r.Id == dto.RegisterId);

            if (registration == null)
                throw new Exception("Không tìm thấy thông tin đăng ký học");

            // Tính danh sách ngày học theo StudyDays
            var studyDates = GenerateStudyDates(registration.StartDateTime, registration.StudyDays, sections.Count);

            // Gắn ngày học cho từng Section
            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                var lessonDate = studyDates.ElementAtOrDefault(i);

                bool exists = await _repository.Get()
                    .AnyAsync(lp => lp.StudentProfileId == dto.StudentId && lp.SectionId == section.Id);

                if (!exists)
                {
                    var lp = new LearningProgress
                    {
                        StudentProfileId = dto.StudentId,
                        CourseId = dto.CourseId,
                        SectionId = section.Id,
                        InstructorProfileId = dto.TeacherId,
                        Comment = "",
                        IsCompleted = false,
                        StartDate = lessonDate != default ? lessonDate : dto.StartDate,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _repository.AddAsync(lp);
                }
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<List<CourseLearningProgressGroupDto>> GetByStudentGroupedAsync(long accountId, bool? isCompleted = null)
        {
            var student = await _studentProfileRepository.Get()
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (student == null)
                throw new NotFoundException("Không tìm thấy hồ sơ học sinh");

            var query = _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.InstructorProfile).ThenInclude(i => i.Account)
                .Include(lp => lp.StudentProfile).ThenInclude(sp => sp.Account)
                .Where(lp => lp.StudentProfileId == student.Id);

            if (isCompleted.HasValue)
                query = query.Where(lp => lp.IsCompleted == isCompleted.Value);

            var grouped = await query
                .OrderBy(lp => lp.StartDate)
                .GroupBy(lp => new { lp.CourseId, lp.Course.Name })
                .ToListAsync();

            var courseIds = grouped.Select(g => g.Key.CourseId).ToList();

            var feedbacks = await _feedbackRepository.Get()
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Ward)
                .Include(f => f.StudentProfile).ThenInclude(sp => sp.Address).ThenInclude(a => a.Province)
                .Include(f => f.InstructorProfile).ThenInclude(ip => ip.Account)
                .Where(f => f.StudentProfileId == student.Id && courseIds.Contains(f.CourseId))
                .ToListAsync();

            var fbMap = feedbacks.ToDictionary(
                f => f.CourseId,
                f => new FeedbackDto
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,

                    Student = new MeDto
                    {
                        AccountId = f.StudentProfile.Account.Id,
                        FullName = f.StudentProfile.Account.FullName,
                        Email = f.StudentProfile.Account.Email,
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
                            ProvinceName = f.StudentProfile.Address.Province.Name
                        }
                    },

                    Instructor = f.InstructorProfile == null ? null : new MeDto
                    {
                        AccountId = f.InstructorProfile.Account.Id,
                        FullName = f.InstructorProfile.Account.FullName,
                        Email = f.InstructorProfile.Account.Email,
                        PhoneNumber = f.InstructorProfile.Account.PhoneNumber,
                        Avatar = f.InstructorProfile.Account.Avatar,
                        LicenseNumber = f.InstructorProfile.LicenseNumber,
                        ExperienceYears = f.InstructorProfile.ExperienceYears
                    }
                }
            );

            var regExamMap = await _registrationExamRepository.Get()
                .Where(r => r.StudentProfileId == student.Id &&
                            r.CourseId.HasValue &&
                            courseIds.Contains(r.CourseId.Value))
                .Select(r => r.CourseId.Value)
                .Distinct()
                .ToDictionaryAsync(cid => cid, cid => true);

            var result = grouped.Select(g => new CourseLearningProgressGroupDto
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.Name,

                Progresses = g.OrderBy(p => p.StartDate)
                    .Select(p => new LearningProgressItemDto
                    {
                        Id = p.Id,
                        SectionId = p.SectionId,
                        SectionName = p.Section.Title,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        IsCompleted = p.IsCompleted,
                        InstructorId = p.InstructorProfileId,
                        InstructorName = p.InstructorProfile != null
                            ? p.InstructorProfile.Account.FullName
                            : null,
                        Comment = p.Comment
                    })
                    .ToList(),

                Feedback = fbMap.ContainsKey(g.Key.CourseId)
                    ? fbMap[g.Key.CourseId]
                    : null,

                IsRegistrationExam = regExamMap.ContainsKey(g.Key.CourseId.Value)
            })
            .ToList();

            return result;
        }


        public async Task<List<CourseProgressGroupDto>> GetAdminLearningProgressAsync(bool? isCompleted = null)
        {
            var query = _repository.Get();


            if (isCompleted.HasValue)
                query = query.Where(lp => lp.IsCompleted == isCompleted.Value);

            var result = await query
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(lp => lp.InstructorProfile).ThenInclude(ip => ip.Account)
                .OrderBy(lp => lp.CourseId)
                .ThenBy(lp => lp.StudentProfileId)
                .ThenBy(lp => lp.StartDate)
                .GroupBy(lp => new { lp.CourseId, lp.Course.Name })
                .Select(courseGroup => new CourseProgressGroupDto
                {
                    CourseId = (long)courseGroup.Key.CourseId,
                    CourseName = courseGroup.Key.Name,

                    Students = courseGroup
                        .GroupBy(lp => new { lp.StudentProfileId, lp.StudentProfile.Account.FullName })
                        .Select(studentGroup => new AdminStudentProgressGroupDto
                        {
                            StudentId = studentGroup.Key.StudentProfileId,
                            StudentName = studentGroup.Key.FullName,

                            InstructorId = studentGroup.First().InstructorProfileId,
                            InstructorName = studentGroup.First().InstructorProfile.Account.FullName,

                            Progresses = studentGroup
                                .OrderBy(lp => lp.StartDate)
                                .Select(lp => new LearningProgressItemDto
                                {
                                    Id = lp.Id,
                                    SectionId = lp.SectionId,
                                    SectionName = lp.Section.Title,
                                    StartDate = lp.StartDate,
                                    EndDate = lp.EndDate,
                                    IsCompleted = lp.IsCompleted,
                                    InstructorId = lp.InstructorProfileId,
                                    InstructorName = lp.InstructorProfile.Account.FullName,
                                    Comment = lp.Comment
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<CourseProgressGroupDto>> GetByInstructorGroupedAsync(
       long instructorAccountId, bool? isCompleted = null)
        {
            var query = _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.StudentProfile).ThenInclude(sp => sp.Account)
                .Include(lp => lp.InstructorProfile).ThenInclude(ip => ip.Account)
                .Where(lp => lp.InstructorProfile.AccountId == instructorAccountId);

            if (isCompleted.HasValue)
                query = query.Where(lp => lp.IsCompleted == isCompleted.Value);

            return await query
                .OrderBy(lp => lp.CourseId)
                .ThenBy(lp => lp.StudentProfileId)
                .ThenBy(lp => lp.StartDate)

                .GroupBy(lp => new { lp.CourseId, lp.Course.Name })
                .Select(courseGroup => new CourseProgressGroupDto
                {
                    CourseId = courseGroup.Key.CourseId ?? 0,
                    CourseName = courseGroup.Key.Name,

                    Students = courseGroup
                        .GroupBy(lp => new { lp.StudentProfileId, lp.StudentProfile.Account.FullName, lp.StudentProfile.Account.Avatar, lp.StudentProfile.Account.Email })
                        .Select(studentGroup => new AdminStudentProgressGroupDto
                        {
                            StudentId = studentGroup.Key.StudentProfileId,
                            StudentName = studentGroup.Key.FullName,
                            StudentAvatar = studentGroup.Key.Avatar ?? "",
                            StudentEmail = studentGroup.Key.Email,

                            Progresses = studentGroup
                                .OrderBy(lp => lp.StartDate)
                                .Select(lp => new LearningProgressItemDto
                                {
                                    Id = lp.Id,
                                    SectionId = lp.SectionId,
                                    SectionName = lp.Section.Title,
                                    StartDate = lp.StartDate,
                                    EndDate = lp.EndDate,
                                    IsCompleted = lp.IsCompleted,
                                    InstructorId = lp.InstructorProfileId,
                                    InstructorName = lp.InstructorProfile.Account.FullName,
                                    Comment = lp.Comment
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        private List<DateTime> GenerateStudyDates(DateTime startDate, StudyDay studyDays, int count)
        {
            var dates = new List<DateTime>();
            var date = startDate.Date;
            int added = 0;
            int safetyCounter = 0; // tránh vòng lặp vô tận

            while (added < count)
            {
                if (date > DateTime.MaxValue.AddDays(-1))
                    throw new Exception("Lịch học vượt quá giới hạn thời gian hợp lệ.");

                safetyCounter++;
                if (safetyCounter > 10000)
                    throw new Exception("Không thể sinh đủ ngày học. Có thể StudyDays không chứa ngày nào hợp lệ.");

                var dayOfWeek = date.DayOfWeek switch
                {
                    DayOfWeek.Monday => StudyDay.Monday,
                    DayOfWeek.Tuesday => StudyDay.Tuesday,
                    DayOfWeek.Wednesday => StudyDay.Wednesday,
                    DayOfWeek.Thursday => StudyDay.Thursday,
                    DayOfWeek.Friday => StudyDay.Friday,
                    DayOfWeek.Saturday => StudyDay.Saturday,
                    DayOfWeek.Sunday => StudyDay.Sunday,
                    _ => StudyDay.None
                };

                if (studyDays.HasFlag(dayOfWeek))
                {
                    dates.Add(date.Add(startDate.TimeOfDay));
                    added++;
                }

                date = date.AddDays(1);
            }

            return dates;
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
            progress.InstructorProfileId = teacher.Id;

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
                if (lp.InstructorProfileId != currentTeacher.Id)
                    throw new Exception($"Bạn không có quyền đổi tiến độ ID {lp.Id}");

                lp.InstructorProfileId = dto.NewStaffId;
                lp.LastUpdated = DateTime.UtcNow;
            }
            await _repository.SaveChangesAsync();
        }

        public async Task<LearningProgressDetailDto> GetByIdAsync(long id)
        {
            var progress = await _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.InstructorProfile)
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

                TeacherId = progress.InstructorProfileId,
                TeacherName = progress.InstructorProfile?.Account?.FullName,
                TeacherEmail = progress.InstructorProfile?.Account?.Email,

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
                .Where(lp => lp.InstructorProfileId == teacher.Id)
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
                    Avatar = student.Student.Account.Avatar
                };

                if (student.IsCompleted)
                    dto.CompletedStudents.Add(info);
                else
                    dto.ActiveStudents.Add(info);
            }

            return dto;
        }

        public async Task<List<CompletedCourseDto>> GetCompletedCoursesByStudentAsync(long accountId)
        {
            var progresses = await _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.InstructorProfile).ThenInclude(s => s.Account)
                .Include(lp => lp.StudentProfile)
                .Where(lp => lp.StudentProfile.AccountId == accountId)
                .ToListAsync();

            if (!progresses.Any())
                return new List<CompletedCourseDto>();

            var result = progresses
                .GroupBy(lp => new { lp.CourseId, lp.Course.Name })
                .Where(g => g.All(x => x.IsCompleted))
                .Select(g =>
                {
                    var lastLesson = g
                        .OrderByDescending(x => x.EndDate ?? x.LastUpdated)
                        .FirstOrDefault();

                    return new CompletedCourseDto
                    {
                        CourseId = g.Key.CourseId,
                        CourseName = g.Key.Name,
                        TotalSections = g.Count(),
                        CompletedDate = lastLesson?.EndDate ?? lastLesson?.LastUpdated ?? DateTime.UtcNow,

                        TeacherId = lastLesson?.InstructorProfileId ?? 0,
                        TeacherName = lastLesson?.InstructorProfile?.Account?.FullName,
                        TeacherEmail = lastLesson?.InstructorProfile?.Account?.Email
                    };
                })
                .OrderByDescending(x => x.CompletedDate)
                .ToList();

            return result;
        }


        public async Task<List<LearningProgressDetailDto>> GetByTeacherAndStudentAsync(long teacherId, long studentId)
        {
            var list = await _repository.Get()
                .Include(lp => lp.Course)
                .Include(lp => lp.Section)
                .Include(lp => lp.InstructorProfile).ThenInclude(s => s.Account)
                .Include(lp => lp.StudentProfile).ThenInclude(sp => sp.Account)
                .Where(lp => lp.InstructorProfileId == teacherId && lp.StudentProfileId == studentId)
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

                TeacherId = lp.InstructorProfileId,
                TeacherName = lp.InstructorProfile?.Account?.FullName,
                TeacherEmail = lp.InstructorProfile?.Account?.Email,

                IsCompleted = lp.IsCompleted,
                Comment = lp.Comment,

                StartDate = lp.StartDate,
                EndDate = lp.EndDate,
                LastUpdated = lp.LastUpdated
            }).ToList();
        }

    public async Task UpdateStudentTrainingAsync(long studentId, UpdateStudentTrainingDto dto, long teacherAccountId)
        {
            var teacher = await _staffRepository.Get()
                .FirstOrDefaultAsync(s => s.AccountId == teacherAccountId);

            if (teacher == null)
                throw new Exception("Tài khoản không thuộc giáo viên hợp lệ");

            var student = await _studentProfileRepository.Get()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                throw new Exception("Không tìm thấy học viên");

            if (dto.DistanceKm.HasValue)
                student.DistanceKm = dto.DistanceKm.Value;

            if (dto.DurationMinutes.HasValue)
                student.DurationMinutes = dto.DurationMinutes.Value;

            await _studentProfileRepository.SaveChangesAsync();
        }
    }
}
