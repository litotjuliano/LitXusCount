import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useVatPercentageSettings } from "../../hook/useVatPercentageSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { VatPercentageItem } from "../../api/settings/vatPercentages";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  name: string;
  percentage: string;
}

const emptyForm: FormState = { name: "", percentage: "" };

const VatPercentageSettingsLayer = () => {
  const { pagedQuery, createMutation, editMutation, deleteMutation } = useVatPercentageSettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Settings.VatPercentage.Create);
  const canEdit = hasPermission(Permissions.Settings.VatPercentage.Edit);
  const canDelete = hasPermission(Permissions.Settings.VatPercentage.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const openAdd = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(true);
  };

  const startEdit = (item: VatPercentageItem) => {
    setEditingId(item.id);
    setForm({ name: item.name, percentage: String(item.percentage) });
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
    const payload = { name: form.name, percentage: Number(form.percentage) };

    if (editingId === null) {
      createMutation.mutate(payload, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingId, payload }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add VAT Percentage" : "Edit VAT Percentage"}</Modal.Title>
        </Modal.Header>
        <form id='vat-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-12'>
                <label className='form-label'>Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required autoFocus />
              </div>
              <div className='col-12'>
                <label className='form-label'>Percentage <span className='text-danger'>*</span></label>
                <input type='number' step='0.01' className='form-control' value={form.percentage} onChange={(e) => setForm({ ...form, percentage: e.target.value })} required />
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='vat-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='VAT Percentage list'
            columns={[
              { key: "name", label: "Name", sortable: true },
              { key: "percentage", label: "Percentage", sortable: true },
              { key: "default", label: "Default" },
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
                <td>{item.percentage}%</td>
                <td>{item.isDefault && <span className='badge bg-primary bg-opacity-10 text-primary'>Default</span>}</td>
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

export default VatPercentageSettingsLayer;
