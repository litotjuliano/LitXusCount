using System.Security.Claims;
using LitXusCount.Application.Admin.Roles;
using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Admin.Roles;

internal sealed class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    private static readonly HashSet<string> CatalogPermissions = new(Permissions.All);

    public async Task<PagedResult<RoleListItemDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = roleManager.Roles;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(r => EF.Functions.Like(r.Name!, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        filtered = query.SortDescending ? filtered.OrderByDescending(r => r.Name) : filtered.OrderBy(r => r.Name);

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(r => new RoleListItemDto(r.Id, r.Name!, r.Name == RoleNames.SuperAdmin))
            .ToListAsync(ct);

        return new PagedResult<RoleListItemDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<RoleDetailDto?> GetAsync(string id, CancellationToken ct = default)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return null;
        }

        return await ToDetailAsync(role);
    }

    public async Task<ServiceResult<RoleDetailDto>> CreateAsync(RoleUpsertDto request, CancellationToken ct = default)
    {
        if (await roleManager.RoleExistsAsync(request.Name))
        {
            return ServiceResult<RoleDetailDto>.Failure($"Role '{request.Name}' already exists.");
        }

        var role = new IdentityRole(request.Name);
        var createResult = await roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
        {
            return ServiceResult<RoleDetailDto>.Failure(string.Join(" ", createResult.Errors.Select(e => e.Description)));
        }

        await ApplyPermissionsAsync(role, request.Permissions);

        return ServiceResult<RoleDetailDto>.Success(await ToDetailAsync(role));
    }

    public async Task<ServiceResult<RoleDetailDto>> EditAsync(string id, RoleUpsertDto request, CancellationToken ct = default)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return ServiceResult<RoleDetailDto>.Failure("Role not found.");
        }

        if (role.Name == RoleNames.SuperAdmin)
        {
            return ServiceResult<RoleDetailDto>.Failure("The SuperAdmin role cannot be edited.");
        }

        if (!string.Equals(role.Name, request.Name, StringComparison.OrdinalIgnoreCase) &&
            await roleManager.RoleExistsAsync(request.Name))
        {
            return ServiceResult<RoleDetailDto>.Failure($"Role '{request.Name}' already exists.");
        }

        role.Name = request.Name;
        await roleManager.UpdateNormalizedRoleNameAsync(role);
        await roleManager.UpdateAsync(role);

        await ApplyPermissionsAsync(role, request.Permissions);

        return ServiceResult<RoleDetailDto>.Success(await ToDetailAsync(role));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string id, CancellationToken ct = default)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return ServiceResult<bool>.Failure("Role not found.");
        }

        if (role.Name == RoleNames.SuperAdmin)
        {
            return ServiceResult<bool>.Failure("The SuperAdmin role cannot be deleted.");
        }

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded
            ? ServiceResult<bool>.Success(true)
            : ServiceResult<bool>.Failure(string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    public IReadOnlyList<string> GetPermissionCatalog() => Permissions.All;

    private async Task ApplyPermissionsAsync(IdentityRole role, IReadOnlyList<string> requestedPermissions)
    {
        var desired = requestedPermissions.Where(p => CatalogPermissions.Contains(p)).ToHashSet();
        var existingClaims = await roleManager.GetClaimsAsync(role);
        var existingPermissionClaims = existingClaims.Where(c => c.Type == Permissions.ClaimType).ToList();

        foreach (var claim in existingPermissionClaims.Where(c => !desired.Contains(c.Value)))
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }

        var existingValues = existingPermissionClaims.Select(c => c.Value).ToHashSet();
        foreach (var permission in desired.Where(p => !existingValues.Contains(p)))
        {
            await roleManager.AddClaimAsync(role, new Claim(Permissions.ClaimType, permission));
        }
    }

    private async Task<RoleDetailDto> ToDetailAsync(IdentityRole role)
    {
        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims.Where(c => c.Type == Permissions.ClaimType).Select(c => c.Value).ToList();
        return new RoleDetailDto(role.Id, role.Name!, role.Name == RoleNames.SuperAdmin, permissions);
    }
}
