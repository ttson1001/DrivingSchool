using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.ExamDto;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

public class ExamService : IExamService
{
    private readonly IRepository<Exam> _repository;
    private readonly IRepository<StudentProfile> _studentRepo;
    private readonly IRepository<RegistrationExam> _registrationExamRepository;

    public ExamService(
        IRepository<Exam> repository,
        IRepository<StudentProfile> studentRepo,
        IRepository<RegistrationExam> registrationExamRepository)
    {
        _repository = repository;
        _studentRepo = studentRepo;
        _registrationExamRepository = registrationExamRepository;
    }

    public async Task<List<ExamDto>> GetAllAsync()
    {
        return await _repository.Get()
            .Include(e => e.Course)
            .OrderBy(e => e.ExamDate)
            .Select(e => new ExamDto
            {
                Id = e.Id,
                ExamCode = e.ExamCode,
                CourseId = e.CourseId,
                CourseName = e.Course.Name,
                Date = e.ExamDate,
                Type = e.Type,
                Location = e.Location
            })
            .ToListAsync();
    }

    public async Task<ExamDto?> GetByIdAsync(long id)
    {
        var exam = await _repository.Get()
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exam == null) return null;

        return new ExamDto
        {
            Id = exam.Id,
            ExamCode = exam.ExamCode,
            CourseId = exam.CourseId,
            CourseName = exam.Course?.Name,
            Date = exam.ExamDate,
            Type = exam.Type,
            Location = exam.Location
        };
    }

    public async Task CreateAsync(CreateExamDto dto)
    {
        var exam = new Exam
        {
            ExamCode = dto.ExamCode,
            CourseId = dto.CourseId,
            ExamDate = dto.Date,
            Type = dto.Type,
            Location = dto.Location
        };

        await _repository.AddAsync(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateExamDto dto)
    {
        var exam = await _repository.Get()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (exam == null)
            throw new Exception("Không tìm thấy kỳ thi.");

        exam.ExamCode = dto.ExamCode;
        exam.ExamDate = dto.Date;
        exam.Type = dto.Type;
        exam.Location = dto.Location;

        _repository.Update(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var exam = await _repository.Get()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (exam == null)
            throw new Exception("Không tìm thấy kỳ thi.");

        _repository.Delete(exam);
        await _repository.SaveChangesAsync();
    }

    public async Task<List<UpcomingExamDto>> GetUpcomingExamsForStudentAsync(long accountId)
    {
        var student = await _studentRepo.Get()
            .FirstOrDefaultAsync(s => s.AccountId == accountId);

        if (student == null)
            throw new Exception("Không tìm thấy hồ sơ học sinh.");

        var registrations = await _registrationExamRepository.Get()
            .Where(r => r.StudentProfileId == student.Id)
            .Include(r => r.Exams)
                .ThenInclude(r => r.Exam)
                .ThenInclude(e => e.Course)
            .ToListAsync();

        var exams = registrations
            .SelectMany(r => r.Exams)
            .Select(x => x.Exam)
            .Where(ex => ex.ExamDate > DateTime.UtcNow)
            .OrderBy(ex => ex.ExamDate)
            .ToList();

        return exams.Select(ex => new UpcomingExamDto
        {
            Id = ex.Id,
            ExamCode = ex.ExamCode,
            CourseId = ex.CourseId,
            CourseName = ex.Course.Name,
            Type = ex.Type,
            TypeName = ex.Type.ToString(),
            ExamDate = ex.ExamDate,
            Location = ex.Location
        }).ToList();
    }
}
