using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Application.Tenants;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Identity;
using LitXusCount.Infrastructure.Persistence;
using LitXusCount.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LitXusCount.Infrastructure.Tenants;

internal sealed class TenantService(
    MasterDbContext masterDb,
    IConfiguration configuration,
    ITenantConnectionCache connectionCache,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IEmailConfigEncryptor emailConfigEncryptor) : ITenantService
{
    public async Task<PagedResult<TenantDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = masterDb.Tenants.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Name, pattern) ||
                EF.Functions.Like(x.Slug, pattern) ||
                EF.Functions.Like(x.ContactEmail ?? "", pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        filtered = query.SortDescending
            ? filtered.OrderByDescending(x => x.Name)
            : filtered.OrderBy(x => x.Name);

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new TenantDto(x.Id, x.Name, x.Slug, x.ContactEmail, x.Notes, x.IsActive, x.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<TenantDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<TenantDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await masterDb.Tenants.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<TenantDto>> CreateAsync(TenantUpsertDto request, CancellationToken ct = default)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await masterDb.Tenants.AnyAsync(x => x.Slug == slug, ct))
            return ServiceResult<TenantDto>.Failure($"Slug '{slug}' is already in use.");

        var connectionString = BuildTenantConnectionString(slug);

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Slug = slug,
            ConnectionString = connectionString,
            ContactEmail = request.ContactEmail?.Trim(),
            Notes = request.Notes?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        masterDb.Tenants.Add(tenant);
        await masterDb.SaveChangesAsync(ct);

        await ProvisionTenantDatabaseAsync(connectionString, ct);

        if (!string.IsNullOrWhiteSpace(request.AdminEmail) && !string.IsNullOrWhiteSpace(request.AdminPassword))
        {
            var identityResult = await ProvisionAdminUserAsync(tenant.Id, request.AdminEmail.Trim(), request.AdminPassword, ct);
            if (!identityResult.Succeeded)
                return ServiceResult<TenantDto>.Failure(string.Join(" ", identityResult.Errors.Select(e => e.Description)));
        }

        return ServiceResult<TenantDto>.Success(ToDto(tenant));
    }

    public async Task<ServiceResult<TenantDto>> EditAsync(long id, TenantUpsertDto request, CancellationToken ct = default)
    {
        var entity = await masterDb.Tenants.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return ServiceResult<TenantDto>.Failure("Tenant not found.");

        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await masterDb.Tenants.AnyAsync(x => x.Slug == slug && x.Id != id, ct))
            return ServiceResult<TenantDto>.Failure($"Slug '{slug}' is already in use.");

        entity.Name = request.Name.Trim();
        entity.Slug = slug;
        entity.ContactEmail = request.ContactEmail?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.ModifiedAt = DateTime.UtcNow;

        await masterDb.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(request.AdminEmail) && !string.IsNullOrWhiteSpace(request.AdminPassword))
        {
            var identityResult = await ResetAdminPasswordAsync(entity.Id, request.AdminEmail.Trim(), request.AdminPassword);
            if (!identityResult.Succeeded)
                return ServiceResult<TenantDto>.Failure(string.Join(" ", identityResult.Errors.Select(e => e.Description)));
        }

        return ServiceResult<TenantDto>.Success(ToDto(entity));
    }

    public async Task<bool> ToggleActiveAsync(long id, CancellationToken ct = default)
    {
        var entity = await masterDb.Tenants.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        entity.IsActive = !entity.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        await masterDb.SaveChangesAsync(ct);
        connectionCache.Invalidate(id);
        return true;
    }

    private async Task<IdentityResult> ResetAdminPasswordAsync(long tenantId, string email, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return await ProvisionAdminUserAsync(tenantId, email, newPassword, CancellationToken.None);

        if (user.TenantId != tenantId)
            return IdentityResult.Failed(new IdentityError { Description = $"User '{email}' does not belong to this tenant." });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return await userManager.ResetPasswordAsync(user, token, newPassword);
    }

    private async Task<IdentityResult> ProvisionAdminUserAsync(long tenantId, string email, string password, CancellationToken ct)
    {
        if (!await roleManager.RoleExistsAsync(RoleNames.Admin))
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = "Admin",
            TenantId = tenantId,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, RoleNames.Admin);

        return result;
    }

    private async Task ProvisionTenantDatabaseAsync(string connectionString, CancellationToken ct)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var tenantDb = new ApplicationDbContext(options);
        await tenantDb.Database.MigrateAsync(ct);
        await SystemSettingsSeeder.SeedAsync(tenantDb, emailConfigEncryptor, ct);
    }

    private string BuildTenantConnectionString(string slug)
    {
        var masterCs = configuration.GetConnectionString("DefaultConnection")!;
        var csb = new NpgsqlConnectionStringBuilder(masterCs)
        {
            Database = $"litxuscount_{slug}"
        };
        return csb.ConnectionString;
    }

    private static TenantDto ToDto(Tenant x) =>
        new(x.Id, x.Name, x.Slug, x.ContactEmail, x.Notes, x.IsActive, x.CreatedAt);
}
