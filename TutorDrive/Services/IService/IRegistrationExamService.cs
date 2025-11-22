using TutorDrive.Dtos.Common;
using TutorDrive.Dtos.RegistrationExam;

namespace TutorDrive.Services.IService
{
    public interface IRegistrationExamService
    {
        Task<RegistrationExamDto> SubmitAsync(RegistrationExamCreateDto dto, long accountId);

        Task<PagedResult<RegistrationExamDto>> SearchAsync(RegistrationExamSearchRequest request);

        Task<RegistrationExamDto?> GetByIdAsync(long id);

        Task<List<RegistrationExamDto>> GetMyRegistrationsAsync(long accountId);

        Task UpdateStatusAsync(RegistrationExamStatusUpdateDto dto, long adminAccountId);

        Task<RegistrationExamDto> UpdateByStudentAsync(RegistrationExamUpdateDto dto, long accountId);
    }
}
