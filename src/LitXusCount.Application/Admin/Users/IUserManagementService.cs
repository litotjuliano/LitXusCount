using LitXusCount.Application.Common;

namespace LitXusCount.Application.Admin.Users;

public interface IUserManagementService
{
    Task<PagedResult<UserDto>> ListAsync(PagedQuery query, bool includeInactive, CancellationToken ct = default);
    Task<UserDto?> GetAsync(string id, CancellationToken ct = default);
    Task<ServiceResult<UserDto>> CreateAsync(UserCreateDto request, CancellationToken ct = default);
    Task<ServiceResult<UserDto>> EditAsync(string id, UserEditDto request, CancellationToken ct = default);
    Task<ServiceResult<bool>> DeactivateAsync(string id, CancellationToken ct = default);
    Task<ServiceResult<bool>> ReactivateAsync(string id, CancellationToken ct = default);
    Task<ServiceResult<bool>> AdminResetPasswordAsync(string id, string newPassword, CancellationToken ct = default);
}
