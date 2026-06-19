import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { useCurrencySettings } from "../../hook/useCurrencySettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { CurrencyItem } from "../../api/settings/currencies";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  name: string;
  code: string;
  symbol: string;
  country: string;
  description: string;
}

const emptyForm: FormState = { name: "", code: "", symbol: "", country: "", description: "" };

const CurrencySettingsLayer = () => {
  const { pagedQuery, createMutation, editMutation, deleteMutation } = useCurrencySettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission(Permissions.Settings.Currency.Create);
  const canEdit = hasPermission(Permissions.Settings.Currency.Edit);
  const canDelete = hasPermission(Permissions.Settings.Currency.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const resetForm = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = {
      name: form.name,
      code: form.code,
      symbol: form.symbol || null,
      country: form.country || null,
      description: form.description || null,
    };

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

  const startEdit = (item: CurrencyItem) => {
    setEditingId(item.id);
    setForm({
      name: item.name,
      code: item.code,
      symbol: item.symbol ?? "",
      country: item.country ?? "",
      description: item.description ?? "",
    });
    setError(null);
  };

  const showForm = (editingId === null && canCreate) || (editingId !== null && canEdit);

  return (
    <div className='row gy-4'>
      {showForm && (
      <div className='col-12'>
        <div className='card'>
          <div className='card-header'>
            <h6 className='card-title mb-0'>{editingId === null ? "Add Currency" : "Edit Currency"}</h6>
          </div>
          <div className='card-body'>
            <form onSubmit={handleSubmit}>
              <div className='row gy-3'>
                <div className='col-md-3'>
                  <label className='form-label'>Name</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.name}
                    onChange={(e) => setForm({ ...form, name: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-2'>
                  <label className='form-label'>Code</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.code}
                    onChange={(e) => setForm({ ...form, code: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-2'>
                  <label className='form-label'>Symbol</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.symbol}
                    onChange={(e) => setForm({ ...form, symbol: e.target.value })}
                  />
                </div>
                <div className='col-md-2'>
                  <label className='form-label'>Country</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.country}
                    onChange={(e) => setForm({ ...form, country: e.target.value })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>Description</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.description}
                    onChange={(e) => setForm({ ...form, description: e.target.value })}
                  />
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
          title='Currency list'
          columns={[
            { key: "name", label: "Name", sortable: true },
            { key: "code", label: "Code", sortable: true },
            { key: "symbol", label: "Symbol" },
            { key: "country", label: "Country", sortable: true },
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
              <td>{item.code}</td>
              <td>{item.symbol ?? "—"}</td>
              <td>{item.country ?? "—"}</td>
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

export default CurrencySettingsLayer;
