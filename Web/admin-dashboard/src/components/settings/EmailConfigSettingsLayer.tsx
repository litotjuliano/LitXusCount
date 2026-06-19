import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { useEmailConfigSettings } from "../../hook/useEmailConfigSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { EmailConfigItem } from "../../api/settings/emailConfigs";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  email: string;
  password: string;
  hostname: string;
  port: string;
  sslEnabled: boolean;
  senderFullName: string;
}

const emptyForm: FormState = {
  email: "",
  password: "",
  hostname: "",
  port: "587",
  sslEnabled: true,
  senderFullName: "",
};

const EmailConfigSettingsLayer = () => {
  const { pagedQuery, createMutation, editMutation, deleteMutation } = useEmailConfigSettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const canCreate = hasPermission(Permissions.Settings.EmailConfig.Create);
  const canEdit = hasPermission(Permissions.Settings.EmailConfig.Edit);
  const canDelete = hasPermission(Permissions.Settings.EmailConfig.Delete);
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
      email: form.email,
      password: form.password,
      hostname: form.hostname,
      port: Number(form.port),
      sslEnabled: form.sslEnabled,
      senderFullName: form.senderFullName || null,
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

  const startEdit = (item: EmailConfigItem) => {
    setEditingId(item.id);
    setForm({
      email: item.email,
      password: "",
      hostname: item.hostname,
      port: String(item.port),
      sslEnabled: item.sslEnabled,
      senderFullName: item.senderFullName ?? "",
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
            <h6 className='card-title mb-0'>{editingId === null ? "Add Email Config" : "Edit Email Config"}</h6>
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
                  <label className='form-label'>
                    Password {editingId !== null && <span className='text-secondary-light'>(re-enter to update)</span>}
                  </label>
                  <input
                    type='password'
                    className='form-control'
                    value={form.password}
                    onChange={(e) => setForm({ ...form, password: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Sender name</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.senderFullName}
                    onChange={(e) => setForm({ ...form, senderFullName: e.target.value })}
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>SMTP host</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.hostname}
                    onChange={(e) => setForm({ ...form, hostname: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-2'>
                  <label className='form-label'>Port</label>
                  <input
                    type='number'
                    className='form-control'
                    value={form.port}
                    onChange={(e) => setForm({ ...form, port: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-3 d-flex align-items-center'>
                  <div className='form-check mt-24'>
                    <input
                      type='checkbox'
                      className='form-check-input'
                      id='sslEnabled'
                      checked={form.sslEnabled}
                      onChange={(e) => setForm({ ...form, sslEnabled: e.target.checked })}
                    />
                    <label className='form-check-label' htmlFor='sslEnabled'>
                      SSL enabled
                    </label>
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
      )}

      <div className='col-12'>
        <PaginatedTable
          title='Email Config list'
          columns={[
            { key: "email", label: "Email", sortable: true },
            { key: "hostname", label: "SMTP host", sortable: true },
            { key: "port", label: "Port" },
            { key: "ssl", label: "SSL" },
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
              <td>{item.email}</td>
              <td>{item.hostname}</td>
              <td>{item.port}</td>
              <td>{item.sslEnabled ? "Yes" : "No"}</td>
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

export default EmailConfigSettingsLayer;
