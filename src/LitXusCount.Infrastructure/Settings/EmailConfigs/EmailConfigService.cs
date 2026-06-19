using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.EmailConfigs;

internal sealed class EmailConfigService(ApplicationDbContext db, IEmailConfigEncryptor encryptor) : IEmailConfigService
{
    private static readonly string[] SortableColumns = ["email", "hostname"];

    public async Task<PagedResult<EmailConfigDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.EmailConfigs.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x => EF.Functions.Like(x.Email, pattern) || EF.Functions.Like(x.Hostname, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "email";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("hostname", true) => filtered.OrderByDescending(x => x.Hostname),
            ("hostname", false) => filtered.OrderBy(x => x.Hostname),
            (_, true) => filtered.OrderByDescending(x => x.Email),
            (_, false) => filtered.OrderBy(x => x.Email),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new EmailConfigDto(x.Id, x.Email, x.Hostname, x.Port, x.SslEnabled, x.SenderFullName, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<EmailConfigDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<EmailConfigDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.EmailConfigs
            .Where(x => x.IsActive)
            .OrderBy(x => x.Email)
            .Take(100)
            .Select(x => new EmailConfigDto(x.Id, x.Email, x.Hostname, x.Port, x.SslEnabled, x.SenderFullName, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

    public async Task<EmailConfigDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.EmailConfigs.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<EmailConfigDto>> CreateAsync(EmailConfigUpsertDto request, CancellationToken ct = default)
    {
        var entity = new EmailConfig
        {
            Email = request.Email,
            PasswordEncrypted = encryptor.Encrypt(request.Password),
            Hostname = request.Hostname,
            Port = request.Port,
            SslEnabled = request.SslEnabled,
            SenderFullName = request.SenderFullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.EmailConfigs.Add(entity);
        await db.SaveChangesAsync(ct);

        return ServiceResult<EmailConfigDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<EmailConfigDto>> EditAsync(long id, EmailConfigUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.EmailConfigs.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<EmailConfigDto>.Failure("Not found.");
        }

        entity.Email = request.Email;
        entity.PasswordEncrypted = encryptor.Encrypt(request.Password);
        entity.Hostname = request.Hostname;
        entity.Port = request.Port;
        entity.SslEnabled = request.SslEnabled;
        entity.SenderFullName = request.SenderFullName;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<EmailConfigDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.EmailConfigs.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static EmailConfigDto ToDto(EmailConfig entity) =>
        new(entity.Id, entity.Email, entity.Hostname, entity.Port, entity.SslEnabled, entity.SenderFullName, entity.IsDefault, entity.IsActive);
}
