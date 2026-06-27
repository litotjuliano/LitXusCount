import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useRoleManagement } from "../../hook/useRoleManagement";
import { usePermissions } from "../../hook/usePermissions";
import { useRole } from "../../hook/useRole";
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
  const { isSuperAdmin } = useRole();
  const { pagedQuery, catalogQuery, createMutation, editMutation, deleteMutation } = useRoleManagement();

  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Roles.Create);
  const canEdit = hasPermission(Permissions.Roles.Edit);
  const canDelete = hasPermission(Permissions.Roles.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const openAdd = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(true); };

  const startEdit = async (role: RoleListItem) => {
    setError(null);
    try {
      const detail = await rolesApi.get(role.id);
      setEditingId(detail.id);
      setForm({ name: detail.name, permissions: detail.permissions });
      setShowModal(true);
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  };

  const closeModal = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(false); };

  const togglePermission = (permission: string) => {
    setForm((prev) => ({
      ...prev,
      permissions: prev.permissions.includes(permission)
        ? prev.permissions.filter((p) => p !== permission)
        : [...prev.permissions, permission],
    }));
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    const payload = { name: form.name, permissions: form.permissions };
    if (editingId === null) {
      createMutation.mutate(payload, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingId, payload }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  const groups = catalogQuery.data ? groupCatalog(catalogQuery.data) : new Map<string, string[]>();

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered size='xl' scrollable>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add Role" : "Edit Role"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form id='role-form' onSubmit={handleSubmit}>
            <div className='row gy-3'>
              <div className='col-md-6'>
                <label className='form-label'>Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required autoFocus />
              </div>
            </div>
            <div className='mt-3'>
              <label className='form-label'>Permissions</label>
              <div className='row gy-3'>
                {[...groups.entries()].map(([groupKey, permissions]) => (
                  <div className='col-md-4' key={groupKey}>
                    <div className='border rounded p-2'>
                      <div className='fw-semibold mb-2 small'>{groupKey}</div>
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
                            <label className='form-check-label small' htmlFor={`perm-${permission}`}>{action}</label>
                          </div>
                        );
                      })}
                    </div>
                  </div>
                ))}
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </form>
        </Modal.Body>
        <Modal.Footer>
          <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
          <button type='submit' form='role-form' className='btn btn-primary' disabled={isSaving}>
            {editingId === null ? "Add" : "Save"}
          </button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Role list'
            columns={[
              { key: "name", label: "Name", sortable: true },
              { key: "actions", label: "Action" },
            ]}
            items={(() => {
              const raw = pagedQuery.result.data?.items ?? [];
              return isSuperAdmin ? raw : raw.filter(r => r.name !== 'SuperAdmin');
            })()}
            totalCount={(() => {
              const raw = pagedQuery.result.data?.items ?? [];
              const hidden = isSuperAdmin ? 0 : raw.filter(r => r.name === 'SuperAdmin').length;
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
            renderRow={(role) => (
              <tr key={role.id}>
                <td>
                  {role.name}
                  {role.isProtected && <span className='badge bg-primary bg-opacity-10 text-primary ms-2'>Protected</span>}
                </td>
                <td className='text-center'>
                  {canEdit && !role.isProtected && (
                    <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(role)}>
                      <Icon icon='lucide:edit' />
                    </button>
                  )}
                  {canDelete && !role.isProtected && (
                    <button type='button' className='btn btn-icon btn-soft-danger' onClick={() => deleteMutation.mutate(role.id)}>
                      <Icon icon='mingcute:delete-2-line' />
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

export default RoleManagementLayer;
