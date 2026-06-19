using LitXusCount.Application.Common;

namespace LitXusCount.Application.Admin.Roles;

public interface IRoleService
{
    Task<PagedResult<RoleListItemDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<RoleDetailDto?> GetAsync(string id, CancellationToken ct = default);
    Task<ServiceResult<RoleDetailDto>> CreateAsync(RoleUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<RoleDetailDto>> EditAsync(string id, RoleUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<bool>> DeleteAsync(string id, CancellationToken ct = default);
    IReadOnlyList<string> GetPermissionCatalog();
}
