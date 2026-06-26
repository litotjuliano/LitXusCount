import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useLookupSettings } from "../../hook/useLookupSettings";
import { usePermissions } from "../../hook/usePermissions";
import type { LookupItem } from "../../api/settings/types";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface ResourcePermissions {
  Create: string;
  Edit: string;
  Delete: string;
}

interface LookupSettingsLayerProps {
  resourcePath: string;
  title: string;
  permissions: ResourcePermissions;
}

interface FormState {
  name: string;
  description: string;
}

const emptyForm: FormState = { name: "", description: "" };

const LookupSettingsLayer = ({ resourcePath, title, permissions }: LookupSettingsLayerProps) => {
  const { pagedQuery, createMutation, editMutation, deleteMutation } = useLookupSettings(resourcePath);
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(permissions.Create);
  const canEdit = hasPermission(permissions.Edit);
  const canDelete = hasPermission(permissions.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const openAdd = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(true);
  };

  const startEdit = (item: LookupItem) => {
    setEditingId(item.id);
    setForm({ name: item.name, description: item.description ?? "" });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(false);
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = { name: form.name, description: form.description || null };

    if (editingId === null) {
      createMutation.mutate(payload, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate(
        { id: editingId, payload },
        { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) },
      );
    }
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? `Add ${title}` : `Edit ${title}`}</Modal.Title>
        </Modal.Header>
        <form id='lookup-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-12'>
                <label className='form-label'>Name <span className='text-danger'>*</span></label>
                <input
                  type='text'
                  className='form-control'
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  required
                  autoFocus
                />
              </div>
              <div className='col-12'>
                <label className='form-label'>Description</label>
                <input
                  type='text'
                  className='form-control'
                  value={form.description}
                  onChange={(e) => setForm({ ...form, description: e.target.value })}
                />
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='lookup-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title={`${title} list`}
            columns={[
              { key: "name", label: "Name", sortable: true },
              { key: "description", label: "Description", sortable: true },
              { key: "actions", label: "Action" },
            ]}
            items={pagedQuery.result.data?.items ?? []}
            totalCount={pagedQuery.result.data?.totalCount ?? 0}
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
            renderRow={(item) => (
              <tr key={item.id}>
                <td>{item.name}</td>
                <td>{item.description ?? "—"}</td>
                <td className='text-center'>
                  {canEdit && (
                    <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(item)}>
                      <Icon icon='lucide:edit' />
                    </button>
                  )}
                  {canDelete && (
                    <button type='button' className='btn btn-icon btn-soft-danger' onClick={() => deleteMutation.mutate(item.id)}>
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

export default LookupSettingsLayer;
