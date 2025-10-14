using TutorDrive.Dtos.Role;

namespace TutorDrive.Services.IService
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
    }
}

