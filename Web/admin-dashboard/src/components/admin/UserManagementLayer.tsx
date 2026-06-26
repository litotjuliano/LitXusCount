import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useQuery } from "@tanstack/react-query";
import { useUserManagement } from "../../hook/useUserManagement";
import { rolesApi } from "../../api/admin/roles";
import { usePermissions } from "../../hook/usePermissions";
import { useRole } from "../../hook/useRole";
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
  const { isSuperAdmin } = useRole();
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
  const [showUserModal, setShowUserModal] = useState(false);
  const [resetPasswordUserId, setResetPasswordUserId] = useState<string | null>(null);
  const [newPassword, setNewPassword] = useState("");

  const canCreate = hasPermission(Permissions.Users.Create);
  const canEdit = hasPermission(Permissions.Users.Edit);
  const canDelete = hasPermission(Permissions.Users.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const openAdd = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowUserModal(true); };

  const startEdit = (user: UserItem) => {
    setEditingId(user.id);
    setForm({ email: user.email, displayName: user.displayName ?? "", password: "", roles: user.roles });
    setError(null);
    setShowUserModal(true);
  };

  const closeUserModal = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowUserModal(false); };

  const toggleRole = (roleName: string) => {
    setForm((prev) => ({
      ...prev,
      roles: prev.roles.includes(roleName) ? prev.roles.filter((r) => r !== roleName) : [...prev.roles, roleName],
    }));
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (editingId === null) {
      createMutation.mutate(
        { email: form.email, displayName: form.displayName, password: form.password, roles: form.roles },
        { onSuccess: closeUserModal, onError: (err) => setError(extractErrorMessage(err)) },
      );
    } else {
      editMutation.mutate(
        { id: editingId, payload: { email: form.email, displayName: form.displayName, roles: form.roles } },
        { onSuccess: closeUserModal, onError: (err) => setError(extractErrorMessage(err)) },
      );
    }
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

  const allRoleOptions = allRolesQuery.data?.items ?? [];
  const roleOptions = isSuperAdmin ? allRoleOptions : allRoleOptions.filter((r) => r.name !== 'SuperAdmin');

  return (
    <>
      <Modal show={showUserModal} onHide={closeUserModal} centered size='lg'>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add User" : "Edit User"}</Modal.Title>
        </Modal.Header>
        <form id='user-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-md-6'>
                <label className='form-label'>Email <span className='text-danger'>*</span></label>
                <input type='email' className='form-control' value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required autoFocus />
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Display Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.displayName} onChange={(e) => setForm({ ...form, displayName: e.target.value })} required />
              </div>
              {editingId === null && (
                <div className='col-md-6'>
                  <label className='form-label'>Password <span className='text-danger'>*</span></label>
                  <input type='password' className='form-control' value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} required />
                </div>
              )}
              <div className='col-12'>
                <label className='form-label'>Roles</label>
                <div className='d-flex flex-wrap gap-3'>
                  {roleOptions.map((role) => (
                    <div className='form-check' key={role.id}>
                      <input
                        type='checkbox'
                        className='form-check-input'
                        id={`role-${role.id}`}
                        checked={form.roles.includes(role.name)}
                        onChange={() => toggleRole(role.name)}
                      />
                      <label className='form-check-label' htmlFor={`role-${role.id}`}>{role.name}</label>
                    </div>
                  ))}
                </div>
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeUserModal}>Cancel</button>
            <button type='submit' form='user-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <Modal show={resetPasswordUserId !== null} onHide={() => { setResetPasswordUserId(null); setNewPassword(""); }} centered size='sm'>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>Reset Password</Modal.Title>
        </Modal.Header>
        <form id='reset-pw-form' onSubmit={handleResetPassword}>
          <Modal.Body>
            <label className='form-label'>New Password <span className='text-danger'>*</span></label>
            <input type='password' className='form-control' value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required autoFocus />
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={() => { setResetPasswordUserId(null); setNewPassword(""); }}>Cancel</button>
            <button type='submit' form='reset-pw-form' className='btn btn-primary' disabled={resetPasswordMutation.isPending}>Set Password</button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <div className='form-check mb-2'>
            <input type='checkbox' className='form-check-input' id='includeInactive' checked={includeInactive} onChange={(e) => setIncludeInactive(e.target.checked)} />
            <label className='form-check-label' htmlFor='includeInactive'>Show deactivated users</label>
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
            items={(() => {
              const raw = pagedQuery.result.data?.items ?? [];
              return isSuperAdmin ? raw : raw.filter(u => !u.roles.includes('SuperAdmin'));
            })()}
            totalCount={(() => {
              const raw = pagedQuery.result.data?.items ?? [];
              const hidden = isSuperAdmin ? 0 : raw.filter(u => u.roles.includes('SuperAdmin')).length;
              return Math.max(0, (pagedQuery.result.data?.totalCount ?? 0) - hidden);
            })()}
            page={pagedQuery.page}
            pageSize={pagedQuery.pageSize}
            isLoading={pagedQuery.result.isLoading}
            isError={pagedQuery.result.isError}
            search={pagedQuery.search}
            onSearchChange={pagedQuery.setSearch}
            sortBy={pagedQuery.sortBy}
            sortDescending={pagedQuery.sortDescending}
            onSortChange={pagedQuery.toggleSort}
            onPageChange={pagedQuery.setPage}
            onPageSizeChange={pagedQuery.setPageSize}
            headerAction={canCreate ? (
              <button type='button' className='btn btn-sm btn-primary d-flex align-items-center gap-1' onClick={openAdd}>
                <Icon icon='lucide:plus' width={14} />Add
              </button>
            ) : undefined}
            renderRow={(user) => (
              <tr key={user.id}>
                <td>{user.email}</td>
                <td>{user.displayName ?? "—"}</td>
                <td>{user.roles.join(", ") || "—"}</td>
                <td>
                  {user.isActive
                    ? <span className='badge bg-success bg-opacity-25 text-success'>Active</span>
                    : <span className='badge bg-danger bg-opacity-25 text-danger'>Deactivated</span>}
                </td>
                <td className='text-center'>
                  {canEdit && (
                    <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(user)}>
                      <Icon icon='lucide:edit' />
                    </button>
                  )}
                  {canEdit && (
                    <button type='button' className='btn btn-icon btn-soft-warning me-1' title='Reset Password' onClick={() => setResetPasswordUserId(user.id)}>
                      <Icon icon='mdi:key-outline' />
                    </button>
                  )}
                  {canDelete && user.isActive && (
                    <button type='button' className='btn btn-icon btn-soft-danger' onClick={() => deactivateMutation.mutate(user.id)}>
                      <Icon icon='mingcute:forbid-circle-line' />
                    </button>
                  )}
                  {canEdit && !user.isActive && (
                    <button type='button' className='btn btn-icon btn-soft-success' onClick={() => reactivateMutation.mutate(user.id)}>
                      <Icon icon='mingcute:check-circle-line' />
                    </button>
                  )}
                </td>
              </tr>
            )}
          />
        </div>
      </div>
    </>
  );
};

export default UserManagementLayer;
