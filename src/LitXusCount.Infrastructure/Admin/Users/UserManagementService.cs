using LitXusCount.Application.Admin.Users;
using LitXusCount.Application.Common;
using LitXusCount.Application.Tenants;
using LitXusCount.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Admin.Users;

internal sealed class UserManagementService(
    UserManager<ApplicationUser> userManager,
    ITenantContext tenantContext) : IUserManagementService
{
    public async Task<PagedResult<UserDto>> ListAsync(PagedQuery query, bool includeInactive, CancellationToken ct = default)
    {
        var filtered = userManager.Users;

        // Company Admin sees only their own tenant's users; SuperAdmin sees all
        if (!tenantContext.IsSuperAdmin)
            filtered = filtered.Where(u => u.TenantId == tenantContext.CurrentTenantId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(u => EF.Functions.Like(u.Email ?? "", pattern) || EF.Functions.Like(u.DisplayName ?? "", pattern));
        }

        if (!includeInactive)
        {
            var now = DateTimeOffset.UtcNow;
            filtered = filtered.Where(u => !(u.LockoutEnd.HasValue && u.LockoutEnd > now));
        }

        var totalCount = await filtered.CountAsync(ct);

        filtered = query.SortDescending ? filtered.OrderByDescending(u => u.Email) : filtered.OrderBy(u => u.Email);

        var pageUsers = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .ToListAsync(ct);

        var items = new List<UserDto>();
        foreach (var user in pageUsers)
            items.Add(await ToDtoAsync(user));

        return new PagedResult<UserDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<UserDto?> GetAsync(string id, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return null;

        // Prevent cross-tenant lookup for Company Admin
        if (!tenantContext.IsSuperAdmin && user.TenantId != tenantContext.CurrentTenantId)
            return null;

        return await ToDtoAsync(user);
    }

    public async Task<ServiceResult<UserDto>> CreateAsync(UserCreateDto request, CancellationToken ct = default)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return ServiceResult<UserDto>.Failure($"A user with email '{request.Email}' already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            EmailConfirmed = true,
            TenantId = tenantContext.CurrentTenantId,
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return ServiceResult<UserDto>.Failure(string.Join(" ", createResult.Errors.Select(e => e.Description)));

        if (request.Roles.Count > 0)
            await userManager.AddToRolesAsync(user, request.Roles);

        return ServiceResult<UserDto>.Success(await ToDtoAsync(user));
    }

    public async Task<ServiceResult<UserDto>> EditAsync(string id, UserEditDto request, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return ServiceResult<UserDto>.Failure("User not found.");

        if (!tenantContext.IsSuperAdmin && user.TenantId != tenantContext.CurrentTenantId)
            return ServiceResult<UserDto>.Failure("User not found.");

        user.Email = request.Email;
        user.UserName = request.Email;
        user.DisplayName = request.DisplayName;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return ServiceResult<UserDto>.Failure(string.Join(" ", updateResult.Errors.Select(e => e.Description)));

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(request.Roles, StringComparer.OrdinalIgnoreCase).ToList();
        var rolesToAdd = request.Roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (rolesToRemove.Count > 0) await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (rolesToAdd.Count > 0) await userManager.AddToRolesAsync(user, rolesToAdd);

        return ServiceResult<UserDto>.Success(await ToDtoAsync(user));
    }

    public async Task<ServiceResult<bool>> DeactivateAsync(string id, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return ServiceResult<bool>.Failure("User not found.");

        if (!tenantContext.IsSuperAdmin && user.TenantId != tenantContext.CurrentTenantId)
            return ServiceResult<bool>.Failure("User not found.");

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;
        await userManager.UpdateAsync(user);
        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<bool>> ReactivateAsync(string id, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return ServiceResult<bool>.Failure("User not found.");

        if (!tenantContext.IsSuperAdmin && user.TenantId != tenantContext.CurrentTenantId)
            return ServiceResult<bool>.Failure("User not found.");

        user.LockoutEnd = null;
        await userManager.UpdateAsync(user);
        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<bool>> AdminResetPasswordAsync(string id, string newPassword, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return ServiceResult<bool>.Failure("User not found.");

        if (!tenantContext.IsSuperAdmin && user.TenantId != tenantContext.CurrentTenantId)
            return ServiceResult<bool>.Failure("User not found.");

        await userManager.RemovePasswordAsync(user);
        var result = await userManager.AddPasswordAsync(user, newPassword);
        return result.Succeeded
            ? ServiceResult<bool>.Success(true)
            : ServiceResult<bool>.Failure(string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    private async Task<UserDto> ToDtoAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var isActive = !(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);
        return new UserDto(user.Id, user.Email ?? user.UserName ?? user.Id, user.DisplayName, roles.ToList(), isActive);
    }
}
