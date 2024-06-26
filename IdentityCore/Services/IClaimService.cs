using Microsoft.AspNetCore.Identity;
using IdentityCore.Models.DTOs;

namespace IdentityCore.Services
{
    public interface IClaimService
    {
        Task<IList<ClaimDto>> GetClaimsRoleAsync(string roleName);
        Task<IList<ClaimDto>> GetClaimsUserAsync(string userId);
        Task<IdentityResult> CreateClaimForRoleAsync(ClaimDto claimDto, string roleName);
        Task<IdentityResult> CreateClaimForUserAsync(ClaimDto claimDto, string userId);
        Task<IdentityResult> UpdateClaimForRoleAsync(ClaimDto claimDto, string roleName, string claimType);
        Task<IdentityResult> UpdateClaimForUserAsync(ClaimDto claimDto, string userId, string claimType);
        Task<IdentityResult> UpdateClaimsForRoleAsync(IList<ClaimDto> claimDtos, string roleName);
        Task<IdentityResult> UpdateClaimsForUserAsync(IList<ClaimDto> claimDtos, string userId);
        Task<IdentityResult> DeleteClaimForRoleAsync(ClaimDto claimDto, string roleName);
        Task<IdentityResult> DeleteClaimForUserAsync(ClaimDto claimDto, string userId);
        Task<IdentityResult> DeleteClaimsForUserAsync(string userId);
    }
}
