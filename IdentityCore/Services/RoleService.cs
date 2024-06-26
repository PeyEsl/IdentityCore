using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Models.DTOs;

namespace IdentityCore.Services
{
    public class RoleService : IRoleService
    {
        #region Ctor

        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #endregion

        public async Task<IList<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();

                var roleList = new List<RoleDto>();
                foreach (var role in roles)
                {
                    roleList.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                    });
                }

                return roleList;
            }
            catch
            {
                return new List<RoleDto>();
            }
        }

        public async Task<RoleDto> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    var roleDto = new RoleDto
                    {
                        Id = role!.Id,
                        Name = role.Name,
                    };

                    return roleDto;
                }
                else
                {
                    return new RoleDto();
                }
            }
            catch
            {
                return new RoleDto();
            }
        }
        
        public async Task<RoleDto> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleDto = new RoleDto
                    {
                        Id = role!.Id,
                        Name = role.Name,
                    };

                    return roleDto;
                }
                else
                {
                    return new RoleDto();
                }
            }
            catch
            {
                return new RoleDto();
            }
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            try
            {
                var role = new ApplicationRole { Name = roleName, };

                return await _roleManager.CreateAsync(role);
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> UpdateRoleAsync(RoleDto roleDto)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleDto.Id!);
                if (role == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
                }

                role.Name = roleDto.Name;

                var result = await _roleManager.UpdateAsync(role);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
                }

                var result = await _roleManager.DeleteAsync(role);

                return result;
            }
            catch
            {
                return new IdentityResult();
            }
        }
    }
}
