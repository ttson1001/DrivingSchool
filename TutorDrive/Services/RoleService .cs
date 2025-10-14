using Microsoft.EntityFrameworkCore;
using TutorDrive.Dtos.Role;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepo;

        public RoleService(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepo.Get()
                .OrderBy(r => r.Id)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return roles;
        }
    }
}
