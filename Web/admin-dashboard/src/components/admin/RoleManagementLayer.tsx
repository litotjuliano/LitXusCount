import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { useRoleManagement } from "../../hook/useRoleManagement";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import { rolesApi, type RoleListItem } from "../../api/admin/roles";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";

interface FormState {
  name: string;
  permissions: string[];
}

const emptyForm: FormState = { name: "", permissions: [] };

function groupCatalog(catalog: string[]): Map<string, string[]> {
  const groups = new Map<string, string[]>();
  for (const permission of catalog) {
    const parts = permission.split(".");
    const groupKey = parts.slice(0, -1).join(".");
    const existing = groups.get(groupKey) ?? [];
    existing.push(permission);
    groups.set(groupKey, existing);
  }
  return groups;
}

const RoleManagementLayer = () => {
  const { hasPermission } = usePermissions();
  const { pagedQuery, catalogQuery, createMutation, editMutation, deleteMutation } = useRoleManagement();

  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission(Permissions.Roles.Create);
  const canEdit = hasPermission(Permissions.Roles.Edit);
  const canDelete = hasPermission(Permissions.Roles.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const resetForm = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
  };

  const togglePermission = (permission: string) => {
    setForm((prev) => ({
      ...prev,
      permissions: prev.permissions.includes(permission)
        ? prev.permissions.filter((p) => p !== permission)
        : [...prev.permissions, permission],
    }));
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = { name: form.name, permissions: form.permissions };

    if (editingId === null) {
      createMutation.mutate(payload, { onSuccess: resetForm, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate(
        { id: editingId, payload },
        { onSuccess: resetForm, onError: (err) => setError(extractErrorMessage(err)) },
      );
    }
  };

  const startEdit = async (role: RoleListItem) => {
    setError(null);
    try {
      const detail = await rolesApi.get(role.id);
      setEditingId(detail.id);
      setForm({ name: detail.name, permissions: detail.permissions });
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  };

  const groups = catalogQuery.data ? groupCatalog(catalogQuery.data) : new Map<string, string[]>();

  return (
    <div className='row gy-4'>
      {(canCreate || editingId !== null) && (
        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>{editingId === null ? "Add Role" : "Edit Role"}</h6>
            </div>
            <div className='card-body'>
              <form onSubmit={handleSubmit}>
                <div className='row gy-3'>
                  <div className='col-md-6'>
                    <label className='form-label'>Name</label>
                    <input
                      type='text'
                      className='form-control'
                      value={form.name}
                      onChange={(e) => setForm({ ...form, name: e.target.value })}
                      required
                    />
                  </div>
                </div>

                <div className='mt-16'>
                  <label className='form-label'>Permissions</label>
                  <div className='row gy-3'>
                    {[...groups.entries()].map(([groupKey, permissions]) => (
                      <div className='col-md-4' key={groupKey}>
                        <div className='border rounded p-12'>
                          <div className='fw-semibold mb-8'>{groupKey}</div>
                          {permissions.map((permission) => {
                            const action = permission.split(".").pop();
                            return (
                              <div className='form-check' key={permission}>
                                <input
                                  type='checkbox'
                                  className='form-check-input'
                                  id={`perm-${permission}`}
                                  checked={form.permissions.includes(permission)}
                                  onChange={() => togglePermission(permission)}
                                />
                                <label className='form-check-label' htmlFor={`perm-${permission}`}>
                                  {action}
                                </label>
                              </div>
                            );
                          })}
                        </div>
                      </div>
                    ))}
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
      )}

      <div className='col-12'>
        <PaginatedTable
          title='Role list'
          columns={[
            { key: "name", label: "Name", sortable: true },
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
          renderRow={(role) => (
            <tr key={role.id}>
              <td>
                {role.name}
                {role.isProtected && <span className='badge bg-primary-50 text-primary-600 ms-8'>Protected</span>}
              </td>
              <td className='text-center'>
                {canEdit && !role.isProtected && (
                  <button
                    type='button'
                    className='w-32-px h-32-px me-8 bg-success-focus text-success-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => startEdit(role)}
                  >
                    <Icon icon='lucide:edit' />
                  </button>
                )}
                {canDelete && !role.isProtected && (
                  <button
                    type='button'
                    className='w-32-px h-32-px bg-danger-focus text-danger-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => deleteMutation.mutate(role.id)}
                  >
                    <Icon icon='mingcute:delete-2-line' />
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

export default RoleManagementLayer;
