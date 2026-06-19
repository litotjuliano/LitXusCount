import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
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

  const canCreate = hasPermission(Permissions.Settings.VatPercentage.Create);
  const canEdit = hasPermission(Permissions.Settings.VatPercentage.Edit);
  const canDelete = hasPermission(Permissions.Settings.VatPercentage.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const resetForm = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = { name: form.name, percentage: Number(form.percentage) };

    if (editingId === null) {
      createMutation.mutate(payload, {
        onSuccess: resetForm,
        onError: (err) => setError(extractErrorMessage(err)),
      });
    } else {
      editMutation.mutate(
        { id: editingId, payload },
        {
          onSuccess: resetForm,
          onError: (err) => setError(extractErrorMessage(err)),
        },
      );
    }
  };

  const startEdit = (item: VatPercentageItem) => {
    setEditingId(item.id);
    setForm({ name: item.name, percentage: String(item.percentage) });
    setError(null);
  };

  const showForm = (editingId === null && canCreate) || (editingId !== null && canEdit);

  return (
    <div className='row gy-4'>
      {showForm && (
      <div className='col-12'>
        <div className='card'>
          <div className='card-header'>
            <h6 className='card-title mb-0'>{editingId === null ? "Add VAT Percentage" : "Edit VAT Percentage"}</h6>
          </div>
          <div className='card-body'>
            <form onSubmit={handleSubmit}>
              <div className='row gy-3 align-items-end'>
                <div className='col-md-5'>
                  <label className='form-label'>Name</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.name}
                    onChange={(e) => setForm({ ...form, name: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Percentage</label>
                  <input
                    type='number'
                    step='0.01'
                    className='form-control'
                    value={form.percentage}
                    onChange={(e) => setForm({ ...form, percentage: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-3 d-flex gap-2'>
                  <button type='submit' className='btn btn-primary' disabled={isSaving}>
                    {editingId === null ? "Add" : "Save"}
                  </button>
                  {editingId !== null && (
                    <button type='button' className='btn btn-outline-secondary' onClick={resetForm}>
                      Cancel
                    </button>
                  )}
                </div>
              </div>
              {error && <div className='text-danger-main text-sm mt-2'>{error}</div>}
            </form>
          </div>
        </div>
      </div>
      )}

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
          search={pagedQuery.search}
          onSearchChange={pagedQuery.setSearch}
          sortBy={pagedQuery.sortBy}
          sortDescending={pagedQuery.sortDescending}
          onSortChange={pagedQuery.toggleSort}
          onPageChange={pagedQuery.setPage}
          onPageSizeChange={pagedQuery.setPageSize}
          renderRow={(item) => (
            <tr key={item.id}>
              <td>{item.name}</td>
              <td>{item.percentage}%</td>
              <td>{item.isDefault && <span className='badge bg-primary-50 text-primary-600'>Default</span>}</td>
              <td className='text-center'>
                {canEdit && (
                  <button
                    type='button'
                    className='w-32-px h-32-px me-8 bg-success-focus text-success-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => startEdit(item)}
                  >
                    <Icon icon='lucide:edit' />
                  </button>
                )}
                {canDelete && (
                  <button
                    type='button'
                    className='w-32-px h-32-px bg-danger-focus text-danger-main rounded-circle d-inline-flex align-items-center justify-content-center border-0'
                    onClick={() => deleteMutation.mutate(item.id)}
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

export default VatPercentageSettingsLayer;
