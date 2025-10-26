using Microsoft.EntityFrameworkCore;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly IRepository<LearningProgress> _repository;
        private readonly IRepository<StudentProfile> _studentProfileRepository;
        private readonly IRepository<Section> _sectionRepository;

        public LearningProgressService(
            IRepository<LearningProgress> repository,
            IRepository<StudentProfile> studentProfileRepository,
            IRepository<Section> sectionRepository)
        {
            _repository = repository;
            _studentProfileRepository = studentProfileRepository;
            _sectionRepository = sectionRepository;
        }

        public async Task GenerateProgressForCourseAsync(long accountId, long courseId)
        {
            var studentProfile = await _studentProfileRepository.Get()
                .FirstOrDefaultAsync(sp => sp.AccountId == accountId);

            if (studentProfile == null)
                throw new Exception("StudentProfile không tìm thấy");

            var studentProfileId = studentProfile.Id;

            var sections = await _sectionRepository.Get()
                .Where(s => s.CourseId == courseId)
                .ToListAsync();

            foreach (var section in sections)
            {
                var exists = await _repository.Get()
                    .AnyAsync(lp => lp.StudentProfileId == studentProfileId && lp.SectionId == section.Id);

                if (!exists)
                {
                    var lp = new LearningProgress
                    {
                        StudentProfileId = studentProfileId,
                        CourseId = courseId,
                        SectionId = section.Id,
                        Comment = 0
                    };

                    await _repository.AddAsync(lp);
                }
            }

            await _repository.SaveChangesAsync();
        }
    }
}
