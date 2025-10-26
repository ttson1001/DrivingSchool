using Microsoft.EntityFrameworkCore;
using System;
using TutorDrive.Dtos.Registration;
using TutorDrive.Entities;
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
                profile = new StudentProfile
                {
                    AccountId = accountId,
                    CMND = dto.CCCD,
                    DOB = dto.DOB,
                    Status = "Active"
                };
                await _repositoryStudentProfile.AddAsync(profile);
                await _repositoryStudentProfile.SaveChangesAsync();
            }

            var registration = new Registration
            {
                StudentProfileId = profile.Id,
                CourseId = dto.CourseId,
                Note = dto.Note,
                Status = "Pending",
                RegisterDate = DateTime.UtcNow,
                Files = new List<RegistrationFile>()
            };

                registration.Files.Add(new RegistrationFile { Url = dto.CCCDFront, FileType = Entities.Enum.FileType.CCCD_Front });
                registration.Files.Add(new RegistrationFile { Url = dto.CCCDBack, FileType = Entities.Enum.FileType.CCCD_Back });

            await _repositoryRegistration.AddAsync(registration);
            await _repositoryRegistration.SaveChangesAsync();
        }
    }
}
