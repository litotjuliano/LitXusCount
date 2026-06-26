import { useState, type FormEvent } from "react";
import { Modal } from "react-bootstrap";
import { useAccounts } from "../../hook/useAccounts";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import { Icon } from "@iconify/react/dist/iconify.js";
import type { AccAccountItem } from "../../api/accounts/accounts";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";

interface FormState {
  code: string;
  accountName: string;
  accountNumber: string;
  description: string;
}

const emptyForm: FormState = { code: "", accountName: "", accountNumber: "", description: "" };

const AccountsLayer = () => {
  const { pagedQuery, createMutation, editMutation, deleteMutation } = useAccounts();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingItem, setEditingItem] = useState<AccAccountItem | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<AccAccountItem | null>(null);

  const canCreate = hasPermission(Permissions.Accounts.Account.Create);
  const canEdit = hasPermission(Permissions.Accounts.Account.Edit);
  const canDelete = hasPermission(Permissions.Accounts.Account.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;

  const openAdd = () => { setForm(emptyForm); setEditingItem(null); setError(null); setShowModal(true); };

  const startEdit = (item: AccAccountItem) => {
    setEditingItem(item);
    setForm({ code: item.code, accountName: item.accountName, accountNumber: item.accountNumber ?? "", description: item.description ?? "" });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => { setForm(emptyForm); setEditingItem(null); setError(null); setShowModal(false); };

  const buildPayload = () => ({
    code: form.code.trim(),
    accountName: form.accountName.trim(),
    accountNumber: form.accountNumber.trim() || null,
    description: form.description.trim() || null,
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (editingItem === null) {
      createMutation.mutate(buildPayload(), { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingItem.id, payload: buildPayload() }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  const confirmDelete = () => {
    if (!deleteTarget) return;
    deleteMutation.mutate(deleteTarget.id, { onSuccess: () => setDeleteTarget(null) });
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered size='lg'>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingItem === null ? "Add Account" : "Edit Account"}</Modal.Title>
        </Modal.Header>
        <form id='account-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-md-4'>
                <label className='form-label'>Code <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value })} required maxLength={32} autoFocus />
              </div>
              <div className='col-md-8'>
                <label className='form-label'>Account Name <span className='text-danger'>*</span></label>
                <input type='text' className='form-control' value={form.accountName} onChange={(e) => setForm({ ...form, accountName: e.target.value })} required maxLength={128} />
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Account Number</label>
                <input type='text' className='form-control' value={form.accountNumber} onChange={(e) => setForm({ ...form, accountNumber: e.target.value })} maxLength={64} />
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Description</label>
                <input type='text' className='form-control' value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
              </div>
              {editingItem !== null && (
                <div className='col-12'>
                  <div className='d-flex gap-4 small text-muted p-2 bg-light rounded'>
                    <span>Credit: <strong>{editingItem.credit.toFixed(2)}</strong></span>
                    <span>Debit: <strong>{editingItem.debit.toFixed(2)}</strong></span>
                    <span>Balance: <strong>{editingItem.balance.toFixed(2)}</strong></span>
                  </div>
                </div>
              )}
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='account-form' className='btn btn-primary' disabled={isSaving}>
              {editingItem === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <Modal show={deleteTarget !== null} onHide={() => setDeleteTarget(null)} centered size='sm'>
        <Modal.Body>Delete account <strong>{deleteTarget?.accountName}</strong>? This cannot be undone.</Modal.Body>
        <Modal.Footer>
          <button className='btn btn-outline-secondary btn-sm' onClick={() => setDeleteTarget(null)}>Cancel</button>
          <button className='btn btn-danger btn-sm' disabled={deleteMutation.isPending} onClick={confirmDelete}>Delete</button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Accounts'
            columns={[
              { key: "code", label: "Code", sortable: true },
              { key: "accountName", label: "Account Name", sortable: true },
              { key: "accountNumber", label: "Account No." },
              { key: "credit", label: "Credit" },
              { key: "debit", label: "Debit" },
              { key: "balance", label: "Balance" },
              { key: "actions", label: "Actions" },
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
            renderRow={(item: AccAccountItem) => (
              <tr key={item.id}>
                <td>{item.code}</td>
                <td>{item.accountName}</td>
                <td>{item.accountNumber ?? "—"}</td>
                <td className='text-end'>{item.credit.toFixed(2)}</td>
                <td className='text-end'>{item.debit.toFixed(2)}</td>
                <td className='text-end fw-bold'>{item.balance.toFixed(2)}</td>
                <td className='text-center'>
                  {canEdit && <button type='button' className='btn btn-icon btn-soft-success me-1' onClick={() => startEdit(item)}><Icon icon='lucide:edit' /></button>}
                  {canDelete && <button type='button' className='btn btn-icon btn-soft-danger' onClick={() => setDeleteTarget(item)}><Icon icon='mingcute:delete-2-line' /></button>}
                </td>
              </tr>
            )}
          />
        </div>
      </div>
    </>
  );
};

export default AccountsLayer;
