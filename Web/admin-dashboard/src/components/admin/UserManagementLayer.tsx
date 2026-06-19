import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { useQuery } from "@tanstack/react-query";
import { useUserManagement } from "../../hook/useUserManagement";
import { rolesApi } from "../../api/admin/roles";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { UserItem } from "../../api/admin/users";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";

interface FormState {
  email: string;
  displayName: string;
  password: string;
  roles: string[];
}

const emptyForm: FormState = { email: "", displayName: "", password: "", roles: [] };

const UserManagementLayer = () => {
  const { hasPermission } = usePermissions();
  const [includeInactive, setIncludeInactive] = useState(false);
  const { pagedQuery, createMutation, editMutation, deactivateMutation, reactivateMutation, resetPasswordMutation } =
    useUserManagement(includeInactive);
  const allRolesQuery = useQuery({
    queryKey: ["admin", "roles", "all"],
    queryFn: () => rolesApi.list({ page: 1, pageSize: 100 }),
  });

  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [resetPasswordUserId, setResetPasswordUserId] = useState<string | null>(null);
  const [newPassword, setNewPassword] = useState("");

  const canCreate = hasPermission(Permissions.Users.Create);
  const canEdit = hasPermission(Permissions.Users.Edit);
  const canDelete = hasPermission(Permissions.Users.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const resetForm = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
  };

  const toggleRole = (roleName: string) => {
    setForm((prev) => ({
      ...prev,
      roles: prev.roles.includes(roleName) ? prev.roles.filter((r) => r !== roleName) : [...prev.roles, roleName],
    }));
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);

    if (editingId === null) {
      createMutation.mutate(
        { email: form.email, displayName: form.displayName, password: form.password, roles: form.roles },
        { onSuccess: resetForm, onError: (err) => setError(extractErrorMessage(err)) },
      );
    } else {
      editMutation.mutate(
        { id: editingId, payload: { email: form.email, displayName: form.displayName, roles: form.roles } },
        { onSuccess: resetForm, onError: (err) => setError(extractErrorMessage(err)) },
      );
    }
  };

  const startEdit = (user: UserItem) => {
    setEditingId(user.id);
    setForm({ email: user.email, displayName: user.displayName ?? "", password: "", roles: user.roles });
    setError(null);
  };

  const handleResetPassword = (e: FormEvent) => {
    e.preventDefault();
    if (!resetPasswordUserId) return;
    resetPasswordMutation.mutate(
      { id: resetPasswordUserId, newPassword },
      {
        onSuccess: () => {
          setResetPasswordUserId(null);
          setNewPassword("");
        },
      },
    );
  };

  const roleOptions = allRolesQuery.data?.items ?? [];

  return (
    <div className='row gy-4'>
      {canCreate || editingId !== null ? (
        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>{editingId === null ? "Add User" : "Edit User"}</h6>
            </div>
            <div className='card-body'>
              <form onSubmit={handleSubmit}>
                <div className='row gy-3'>
                  <div className='col-md-4'>
                    <label className='form-label'>Email</label>
                    <input
                      type='email'
                      className='form-control'
                      value={form.email}
                      onChange={(e) => setForm({ ...form, email: e.target.value })}
                      required
                    />
                  </div>
                  <div className='col-md-4'>
                    <label className='form-label'>Display name</label>
                    <input
                      type='text'
                      className='form-control'
                      value={form.displayName}
                      onChange={(e) => setForm({ ...form, displayName: e.target.value })}
                      required
                    />
                  </div>
                  {editingId === null && (
                    <div className='col-md-4'>
                      <label className='form-label'>Password</label>
                      <input
                        type='password'
                        className='form-control'
                        value={form.password}
                        onChange={(e) => setForm({ ...form, password: e.target.value })}
                        required
                      />
                    </div>
                  )}
                  <div className='col-12'>
                    <label className='form-label'>Roles</label>
                    <div className='d-flex flex-wrap gap-12'>
                      {roleOptions.map((role) => (
                        <div className='form-check' key={role.id}>
                          <input
                            type='checkbox'
                            className='form-check-input'
                            id={`role-${role.id}`}
                            checked={form.roles.includes(role.name)}
                            onChange={() => toggleRole(role.name)}
                          />
                          <label className='form-check-label' htmlFor={`role-${role.id}`}>
                            {role.name}
                          </label>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
                <div className='d-flex gap-2 mt-16'>
                  <button type='submit' className='btn btn-primary' disabled={isSaving}>
                    {editingId === null ? "Add" : "Save"}
                  </button>
                  {editingId !== null && (
                    <button type='button' className='btn btn-outline-secondary' onClick={resetForm}>
                      Cancel
                    </button>
                  )}
                </div>
                {error && <div className='text-danger-main text-sm mt-2'>{error}</div>}
              </form>
            </div>
          </div>
        </div>
      ) : null}

      {resetPasswordUserId && (
        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>Reset Password</h6>
            </div>
            <div className='card-body'>
              <form onSubmit={handleResetPassword}>
                <div className='row gy-3 align-items-end'>
                  <div className='col-md-5'>
                    <label className='form-label'>New password</label>
                    <input
                      type='password'
                      className='form-control'
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      required
                    />
                  </div>
                  <div className='col-md-4 d-flex gap-2'>
                    <button type='submit' className='btn btn-primary' disabled={resetPasswordMutation.isPending}>
                      Set Password
                    </button>
                    <button
                      type='button'
                      className='btn btn-outline-secondary'
                      onClick={() => {
                        setResetPasswordUserId(null);
                        setNewPassword("");
                      }}
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      <div className='col-12'>
        <div className='form-check mb-12'>
          <input
            type='checkbox'
            className='form-check-input'
            id='includeInactive'
            checked={includeInactive}
            onChange={(e) => setIncludeInactive(e.target.checked)}
          />
          <label className='form-check-label' htmlFor='includeInactive'>
            Show deactivated users
          </label>
        </div>
        <PaginatedTable
          title='User list'
          columns={[
            { key: "email", label: "Email", sortable: true },
            { key: "displayName", label: "Display Name" },
            { key: "roles", label: "Roles" },
            { key: "status", label: "Status" },
            { key: "actions", label: "Action" },
          ]}
          items={pagedQuery.result.data?.items ?? []}
          totalCount={pagedQuery.result.data?.totalCount ?? 0}
          page={pagedQuery.page}
          pageSize={pagedQuery.pageSize}
          isLoading={pagedQuery.result.isLoading}
          search={pagedQuery.search}
          onSearchChange={pagedQuery.setSearch}
          sortBy={pagedQuery.sortBy}
          sortDescending={pagedQuery.sortDescending}
          onSortChange={pagedQuery.toggleSort}
          onPageChange={pagedQuery.setPage}
          onPageSizeChange={pagedQuery.setPageSize}
          renderRow={(user) => (
            <tr key={user.id}>
              <td>{user.email}</td>
              <td>{user.displayName ?? "—"}</td>
              <td>{user.roles.join(", ") || "—"}</td>
              <td>
                {user.isActive ? (
                  <span className='badge bg-success-focus text-success-main'>Active</span>
                ) : (
                  <span className='badge bg-danger-focus text-danger-main'>Deactivated</span>
                )}
              </td>
              <td className='text-center'>
                {canEdit && (
                  <button
                    type='button'
                    className='w-32-px h-32-px me-8 bg-success-focus text-success-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => startEdit(user)}
                  >
                    <Icon icon='lucide:edit' />
                  </button>
                )}
                {canEdit && (
                  <button
                    type='button'
                    className='w-32-px h-32-px me-8 bg-warning-focus text-warning-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => setResetPasswordUserId(user.id)}
                  >
                    <Icon icon='mdi:key-outline' />
                  </button>
                )}
                {canDelete && user.isActive && (
                  <button
                    type='button'
                    className='w-32-px h-32-px bg-danger-focus text-danger-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => deactivateMutation.mutate(user.id)}
                  >
                    <Icon icon='mingcute:forbid-circle-line' />
                  </button>
                )}
                {canEdit && !user.isActive && (
                  <button
                    type='button'
                    className='w-32-px h-32-px bg-success-focus text-success-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => reactivateMutation.mutate(user.id)}
                  >
                    <Icon icon='mingcute:check-circle-line' />
                  </button>
                )}
              </td>
            </tr>
          )}
        />
      </div>
    </div>
  );
};

export default UserManagementLayer;
