import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useGlAccountSettings } from "../../hook/useGlAccountSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import { AccountTypeLabels, type GlAccountItem } from "../../api/settings/glAccounts";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  code: string; name: string; accountType: number;
  parentId: number | ""; isControl: boolean; openingBalance: number;
}

const emptyForm: FormState = { code: "", name: "", accountType: 1, parentId: "", isControl: false, openingBalance: 0 };

const GlAccountSettingsLayer = () => {
  const { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation } = useGlAccountSettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Settings.GlAccount.Create);
  const canEdit = hasPermission(Permissions.Settings.GlAccount.Edit);
  const canDelete = hasPermission(Permissions.Settings.GlAccount.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;
  const parents = allActiveQuery.data ?? [];

  const openAdd = () => {
    setForm(emptyForm);
    setEditingId(null);
    setError(null);
    setShowModal(true);
  };

  const startEdit = (item: GlAccountItem) => {
    setEditingId(item.id);
    setForm({ code: item.code, name: item.name, accountType: item.accountType, parentId: item.parentId ?? "", isControl: item.isControl, openingBalance: item.openingBalance });
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
    code: form.code, name: form.name, accountType: form.accountType,
    parentId: form.parentId === "" ? null : Number(form.parentId),
    isControl: form.isControl, openingBalance: form.openingBalance,
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (editingId === null) {
      createMutation.mutate(buildPayload(), { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingId, payload: buildPayload() }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered size='lg'>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add GL Account" : "Edit GL Account"}</Modal.Title>
        </Modal.Header>
        <form id='gl-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-md-4'>
                <label className='form-label'>Code <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} required autoFocus />
              </div>
              <div className='col-md-8'>
                <label className='form-label'>Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
              </div>
              <div className='col-md-4'>
                <label className='form-label'>Type</label>
                <select className='form-select' value={form.accountType} onChange={(e) => setForm({ ...form, accountType: Number(e.target.value) })}>
                  {Object.entries(AccountTypeLabels).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
                </select>
              </div>
              <div className='col-md-8'>
                <label className='form-label'>Parent Account</label>
                <select className='form-select' value={form.parentId} onChange={(e) => setForm({ ...form, parentId: e.target.value === "" ? "" : Number(e.target.value) })}>
                  <option value=''>— None —</option>
                  {parents.filter(p => p.id !== editingId).map(p => <option key={p.id} value={p.id}>{p.code} — {p.name}</option>)}
                </select>
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Opening Balance</label>
                <input type='number' className='form-control' step='0.01' value={form.openingBalance} onChange={(e) => setForm({ ...form, openingBalance: parseFloat(e.target.value) || 0 })} />
              </div>
              <div className='col-md-6 d-flex align-items-end'>
                <div className='form-check mb-2'>
                  <input type='checkbox' className='form-check-input' id='isControl' checked={form.isControl} onChange={(e) => setForm({ ...form, isControl: e.target.checked })} />
                  <label className='form-check-label' htmlFor='isControl'>Control Account</label>
                </div>
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='gl-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='GL Accounts'
            columns={[
              { key: "code", label: "Code", sortable: true },
              { key: "name", label: "Name", sortable: true },
              { key: "accountType", label: "Type", sortable: true },
              { key: "parent", label: "Parent" },
              { key: "openingBalance", label: "Opening Bal." },
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
            headerAction={canCreate ? (
              <button type='button' className='btn btn-sm btn-primary d-flex align-items-center gap-1' onClick={openAdd}>
                <Icon icon='lucide:plus' width={14} />Add
              </button>
            ) : undefined}
            renderRow={(item) => (
              <tr key={item.id}>
                <td>{item.code}</td>
                <td>{item.name}</td>
                <td>{AccountTypeLabels[item.accountType] ?? item.accountType}</td>
                <td>{item.parentName ?? "—"}</td>
                <td>{item.openingBalance.toFixed(2)}</td>
                <td className='text-center'>
                  {canEdit && <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(item)}><Icon icon='lucide:edit' /></button>}
                  {canDelete && <button type='button' className='btn btn-icon btn-soft-danger' onClick={() => deleteMutation.mutate(item.id)}><Icon icon='mingcute:delete-2-line' /></button>}
                </td>
              </tr>
            )}
          />
        </div>
      </div>
    </>
  );
};

export default GlAccountSettingsLayer;
