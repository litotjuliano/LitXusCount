import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useTenants } from "../../hook/useTenants";
import type { TenantItem } from "../../api/tenants";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  name: string;
  slug: string;
  contactEmail: string;
  notes: string;
  adminEmail: string;
  adminPassword: string;
}

const emptyForm: FormState = { name: "", slug: "", contactEmail: "", notes: "", adminEmail: "", adminPassword: "" };

const slugify = (value: string) =>
  value.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "");

const TenantManagementLayer = () => {
  const { pagedQuery, createMutation, editMutation, toggleActiveMutation } = useTenants();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const isSaving = createMutation.isPending || editMutation.isPending;
  const n = (v: string) => v.trim() || null;

  const openAdd = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(true);
  };

  const startEdit = (item: TenantItem) => {
    setEditingId(item.id);
    setForm({ name: item.name, slug: item.slug, contactEmail: item.contactEmail ?? "", notes: item.notes ?? "", adminEmail: item.contactEmail ?? "", adminPassword: "" });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(false);
  };

  const buildPayload = () => ({
    name: form.name,
    slug: form.slug,
    contactEmail: n(form.contactEmail),
    notes: n(form.notes),
    adminEmail: n(form.adminEmail),
    adminPassword: n(form.adminPassword),
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (editingId === null) {
      createMutation.mutate(buildPayload(), { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingId, payload: buildPayload() }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  const handleNameChange = (value: string) => {
    setForm((prev) => ({ ...prev, name: value, ...(editingId === null ? { slug: slugify(value) } : {}) }));
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered size='lg'>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Create Tenant" : "Edit Tenant"}</Modal.Title>
        </Modal.Header>
        <form id='tenant-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-md-6'>
                <label className='form-label'>Company Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.name} onChange={(e) => handleNameChange(e.target.value)} required autoFocus />
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Slug <span className='text-danger'>*</span></label>
                <input
                  type='text' className='form-control font-monospace'
                  value={form.slug}
                  onChange={(e) => setForm((p) => ({ ...p, slug: slugify(e.target.value) }))}
                  pattern="[a-z0-9\-]+"
                  required
                />
                <div className='form-text'>Lowercase, hyphens only. Used as DB name: <code>litxuscount_{form.slug || "..."}</code></div>
              </div>
              <div className='col-12'>
                <label className='form-label'>Contact Email</label>
                <input type='email' className='form-control' value={form.contactEmail} onChange={(e) => setForm((p) => ({ ...p, contactEmail: e.target.value }))} />
              </div>
              <div className='col-12'>
                <label className='form-label'>Notes</label>
                <textarea className='form-control' rows={2} value={form.notes} onChange={(e) => setForm((p) => ({ ...p, notes: e.target.value }))} />
              </div>

              {editingId === null ? (
                <>
                  <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Initial Admin Account</p></div>
                  <div className='col-md-6'>
                    <label className='form-label'>Admin Email</label>
                    <input type='email' className='form-control' value={form.adminEmail} onChange={(e) => setForm((p) => ({ ...p, adminEmail: e.target.value }))} placeholder='admin@company.com' />
                    <div className='form-text'>Leave blank to set up later.</div>
                  </div>
                  <div className='col-md-6'>
                    <label className='form-label'>Admin Password</label>
                    <input type='password' className='form-control' value={form.adminPassword} onChange={(e) => setForm((p) => ({ ...p, adminPassword: e.target.value }))} />
                  </div>
                </>
              ) : (
                <>
                  <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Reset Admin Password</p></div>
                  <div className='col-md-6'>
                    <label className='form-label'>Admin Email</label>
                    <input type='email' className='form-control' value={form.adminEmail} onChange={(e) => setForm((p) => ({ ...p, adminEmail: e.target.value }))} placeholder='admin@company.com' />
                  </div>
                  <div className='col-md-6'>
                    <label className='form-label'>New Password</label>
                    <input type='password' className='form-control' value={form.adminPassword} onChange={(e) => setForm((p) => ({ ...p, adminPassword: e.target.value }))} />
                    <div className='form-text'>Leave blank to keep current password.</div>
                  </div>
                </>
              )}
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='tenant-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Create" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Tenants'
            columns={[
              { key: "name", label: "Company Name", sortable: true },
              { key: "slug", label: "Slug" },
              { key: "contactEmail", label: "Contact Email" },
              { key: "status", label: "Status" },
              { key: "createdAt", label: "Created" },
              { key: "actions", label: "Action" },
            ]}
            items={pagedQuery.result.data?.items ?? []}
            totalCount={pagedQuery.result.data?.totalCount ?? 0}
            page={pagedQuery.page} pageSize={pagedQuery.pageSize}
            isLoading={pagedQuery.result.isLoading}
            isError={pagedQuery.result.isError}
            search={pagedQuery.search} onSearchChange={pagedQuery.setSearch}
            sortBy={pagedQuery.sortBy} sortDescending={pagedQuery.sortDescending}
            onSortChange={pagedQuery.toggleSort} onPageChange={pagedQuery.setPage} onPageSizeChange={pagedQuery.setPageSize}
            headerAction={
              <button type='button' className='btn btn-sm btn-primary d-flex align-items-center gap-1' onClick={openAdd}>
                <Icon icon='lucide:plus' width={14} />Add
              </button>
            }
            renderRow={(item) => (
              <tr key={item.id} className={!item.isActive ? "text-muted" : undefined}>
                <td className='fw-medium'>{item.name}</td>
                <td><code className='small'>{item.slug}</code></td>
                <td>{item.contactEmail ?? "—"}</td>
                <td>
                  {item.isActive
                    ? <span className='badge bg-success bg-opacity-25 text-success'>Active</span>
                    : <span className='badge bg-secondary bg-opacity-25 text-secondary'>Inactive</span>}
                </td>
                <td className='small text-muted'>{new Date(item.createdAt).toLocaleDateString()}</td>
                <td className='text-center'>
                  <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(item)}>
                    <Icon icon='lucide:edit' />
                  </button>
                  <button
                    type='button'
                    className={`btn btn-icon ${item.isActive ? "btn-soft-warning" : "btn-soft-primary"}`}
                    title={item.isActive ? "Deactivate" : "Activate"}
                    onClick={() => toggleActiveMutation.mutate(item.id)}
                  >
                    <Icon icon={item.isActive ? "lucide:pause" : "lucide:play"} />
                  </button>
                </td>
              </tr>
            )}
          />
        </div>
      </div>
    </>
  );
};

export default TenantManagementLayer;
