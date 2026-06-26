import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useDeposits } from "../../hook/useDeposits";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { AccDepositItem } from "../../api/accounts/deposits";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";

interface FormState {
  accAccountId: number | "";
  depositDate: string;
  amount: string;
  note: string;
}

const today = () => new Date().toISOString().slice(0, 10);
const emptyForm: FormState = { accAccountId: "", depositDate: today(), amount: "", note: "" };

const DepositsLayer = () => {
  const { pagedQuery, accountsQuery, createMutation, deleteMutation } = useDeposits();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [pendingDeleteId, setPendingDeleteId] = useState<number | null>(null);

  const canCreate = hasPermission(Permissions.Accounts.Deposit.Create);
  const canDelete = hasPermission(Permissions.Accounts.Deposit.Delete);
  const accounts = accountsQuery.data ?? [];

  const openAdd = () => { setForm(emptyForm); setError(null); setShowModal(true); };
  const closeModal = () => { setForm(emptyForm); setError(null); setShowModal(false); };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    createMutation.mutate(
      { accAccountId: Number(form.accAccountId), depositDate: form.depositDate, amount: parseFloat(form.amount), note: form.note.trim() || null },
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
          <Modal.Title className='h6'>Record Deposit</Modal.Title>
        </Modal.Header>
        <form id='deposit-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-12'>
                <label className='form-label'>Account <span className='text-danger'>*</span></label>
                <select className='form-select' value={form.accAccountId} onChange={(e) => setForm({ ...form, accAccountId: Number(e.target.value) })} required autoFocus>
                  <option value=''>— Select Account —</option>
                  {accounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.accountName}</option>)}
                </select>
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Date <span className='text-danger'>*</span></label>
                <input type='date' className='form-control' value={form.depositDate} onChange={(e) => setForm({ ...form, depositDate: e.target.value })} required />
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
            <button type='submit' form='deposit-form' className='btn btn-primary' disabled={createMutation.isPending}>Add Deposit</button>
          </Modal.Footer>
        </form>
      </Modal>

      <Modal show={pendingDeleteId !== null} onHide={() => setPendingDeleteId(null)} centered size='sm'>
        <Modal.Body>This will reverse the deposit and restore the account balance. Continue?</Modal.Body>
        <Modal.Footer>
          <button className='btn btn-outline-secondary btn-sm' onClick={() => setPendingDeleteId(null)}>Cancel</button>
          <button className='btn btn-danger btn-sm' disabled={deleteMutation.isPending} onClick={confirmDelete}>Yes, Reverse</button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Deposits'
            columns={[
              { key: "account", label: "Account" },
              { key: "depositDate", label: "Date", sortable: true },
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
            renderRow={(item: AccDepositItem) => (
              <tr key={item.id}>
                <td>{item.accountName}</td>
                <td>{item.depositDate.slice(0, 10)}</td>
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

export default DepositsLayer;
