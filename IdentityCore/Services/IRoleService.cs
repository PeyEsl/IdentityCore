using Microsoft.AspNetCore.Identity;
using IdentityCore.Models.DTOs;

namespace IdentityCore.Services
{
    public interface IRoleService
    {
        Task<IList<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(string roleId);
        Task<RoleDto> GetRoleByNameAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(RoleDto roleDto);
        Task<IdentityResult> DeleteRoleAsync(string roleId);
    }
}
