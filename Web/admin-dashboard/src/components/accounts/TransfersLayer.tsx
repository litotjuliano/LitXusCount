import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useTransfers } from "../../hook/useTransfers";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { AccTransferItem } from "../../api/accounts/transfers";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";

interface FormState {
  senderAccountId: number | "";
  receiverAccountId: number | "";
  transferDate: string;
  amount: string;
  note: string;
}

const today = () => new Date().toISOString().slice(0, 10);
const emptyForm: FormState = { senderAccountId: "", receiverAccountId: "", transferDate: today(), amount: "", note: "" };

const TransfersLayer = () => {
  const { pagedQuery, accountsQuery, createMutation, deleteMutation } = useTransfers();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [pendingDeleteId, setPendingDeleteId] = useState<number | null>(null);

  const canCreate = hasPermission(Permissions.Accounts.Transfer.Create);
  const canDelete = hasPermission(Permissions.Accounts.Transfer.Delete);
  const accounts = accountsQuery.data ?? [];

  const sameAccount =
    form.senderAccountId !== "" &&
    form.receiverAccountId !== "" &&
    form.senderAccountId === form.receiverAccountId;

  const openAdd = () => { setForm(emptyForm); setError(null); setShowModal(true); };
  const closeModal = () => { setForm(emptyForm); setError(null); setShowModal(false); };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (sameAccount) return;
    createMutation.mutate(
      { senderAccountId: Number(form.senderAccountId), receiverAccountId: Number(form.receiverAccountId), transferDate: form.transferDate, amount: parseFloat(form.amount), note: form.note.trim() || null },
      { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) }
    );
  };

  const confirmDelete = () => {
    if (pendingDeleteId === null) return;
    deleteMutation.mutate(pendingDeleteId, { onSuccess: () => setPendingDeleteId(null) });
  };

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>Record Transfer</Modal.Title>
        </Modal.Header>
        <form id='transfer-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-12'>
                <label className='form-label'>From Account <span className='text-danger'>*</span></label>
                <select className='form-select' value={form.senderAccountId} onChange={(e) => setForm({ ...form, senderAccountId: Number(e.target.value) })} required autoFocus>
                  <option value=''>— Select Account —</option>
                  {accounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.accountName}</option>)}
                </select>
              </div>
              <div className='col-12'>
                <label className='form-label'>To Account <span className='text-danger'>*</span></label>
                <select className={`form-select${sameAccount ? ' is-invalid' : ''}`} value={form.receiverAccountId} onChange={(e) => setForm({ ...form, receiverAccountId: Number(e.target.value) })} required>
                  <option value=''>— Select Account —</option>
                  {accounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.accountName}</option>)}
                </select>
                {sameAccount && <div className='invalid-feedback d-block'>Cannot transfer to the same account.</div>}
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Date <span className='text-danger'>*</span></label>
                <input type='date' className='form-control' value={form.transferDate} onChange={(e) => setForm({ ...form, transferDate: e.target.value })} required />
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Amount <span className='text-danger'>*</span></label>
                <input type='number' className='form-control' step='0.01' min='0.01' value={form.amount} onChange={(e) => setForm({ ...form, amount: e.target.value })} required />
              </div>
              <div className='col-12'>
                <label className='form-label'>Note</label>
                <input type='text' className='form-control' value={form.note} onChange={(e) => setForm({ ...form, note: e.target.value })} />
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='transfer-form' className='btn btn-primary' disabled={createMutation.isPending || sameAccount}>Save</button>
          </Modal.Footer>
        </form>
      </Modal>

      <Modal show={pendingDeleteId !== null} onHide={() => setPendingDeleteId(null)} centered size='sm'>
        <Modal.Body>This will reverse the transfer and restore both account balances. Continue?</Modal.Body>
        <Modal.Footer>
          <button className='btn btn-outline-secondary btn-sm' onClick={() => setPendingDeleteId(null)}>Cancel</button>
          <button className='btn btn-danger btn-sm' disabled={deleteMutation.isPending} onClick={confirmDelete}>Yes, Reverse</button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Transfers'
            columns={[
              { key: "from", label: "From" },
              { key: "to", label: "To" },
              { key: "transferDate", label: "Date", sortable: true },
              { key: "amount", label: "Amount" },
              { key: "note", label: "Note" },
              { key: "actions", label: "" },
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
            renderRow={(item: AccTransferItem) => (
              <tr key={item.id}>
                <td>{item.senderAccountName}</td>
                <td>{item.receiverAccountName}</td>
                <td>{item.transferDate.slice(0, 10)}</td>
                <td className='text-end'>{item.amount.toFixed(2)}</td>
                <td>{item.note ?? "—"}</td>
                <td>
                  {canDelete && <button className='btn btn-sm btn-soft-danger' onClick={() => setPendingDeleteId(item.id)}>Reverse</button>}
                </td>
              </tr>
            )}
          />
        </div>
      </div>
    </>
  );
};

export default TransfersLayer;
