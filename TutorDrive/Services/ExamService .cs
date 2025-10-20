namespace TutorDrive.Services
{
    using global::TutorDrive.Dtos.ExamDto;
    using global::TutorDrive.Entities;
    using global::TutorDrive.Repositories;
    using global::TutorDrive.Services.IService;
    using Microsoft.EntityFrameworkCore;

    public class ExamService : IExamService
    {
        private readonly IRepository<Exam> _repository;

        public ExamService(IRepository<Exam> repository)
        {
            _repository = repository;
        }

        public async Task<List<ExamDto>> GetAllAsync()
        {
            return await _repository.Get()
                .Include(e => e.Course)
                .Select(e => new ExamDto
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name,
                    Date = e.Date,
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
                CourseId = exam.CourseId,
                CourseName = exam.Course?.Name,
                Date = exam.Date,
                Type = exam.Type,
                Location = exam.Location
            };
        }

        public async Task CreateAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                CourseId = dto.CourseId,
                Date = dto.Date,
                Type = dto.Type,
                Location = dto.Location
            };

            await _repository.AddAsync(exam);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateExamDto dto)
        {
            var exam = await _repository.Get().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (exam == null) throw new Exception("Không tìm thấy kỳ thi.");

            exam.Date = dto.Date;
            exam.Type = dto.Type;
            exam.Location = dto.Location;

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var exam = await _repository.Get().FirstOrDefaultAsync(x => x.Id == id);
            if (exam == null) throw new Exception("Không tìm thấy kỳ thi.");

            _repository.Delete(exam);
            await _repository.SaveChangesAsync();
        }
    }
}


