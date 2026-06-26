import { useState } from "react";
import { Modal } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useSalesInvoices } from "../../hook/useSalesInvoices";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import { useQuery } from "@tanstack/react-query";
import { customersApi } from "../../api/settings/customers";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import PaginatedTable from "../settings/PaginatedTable";
import type { SalesInvoiceCategory, SalesInvoiceItem } from "../../api/sales/salesInvoices";

const CATEGORY_LABELS: Record<number, string> = { 1: "Regular", 2: "Draft", 3: "Quote", 4: "Manual" };
const STATUS_LABELS: Record<number, string> = { 0: "Unpaid", 1: "Partial", 2: "Paid" };
const STATUS_BADGES: Record<number, string> = {
  0: "badge bg-danger bg-opacity-25 text-danger",
  1: "badge bg-warning bg-opacity-25 text-warning",
  2: "badge bg-success bg-opacity-25 text-success",
};

interface FormState {
  customerId: number | "";
}

const emptyForm: FormState = { customerId: "" };

const SalesInvoicesLayer = () => {
  const navigate = useNavigate();
  const { hasPermission } = usePermissions();
  const [categoryFilter, setCategoryFilter] = useState<SalesInvoiceCategory | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);
  const [pendingDeleteId, setPendingDeleteId] = useState<number | null>(null);

  const { pagedQuery, createMutation, deleteMutation } = useSalesInvoices(categoryFilter);
  const customersQuery = useQuery({ queryKey: ["settings", "customers", "all-active"], queryFn: customersApi.listAllActive });
  const customers = customersQuery.data ?? [];

  const canCreate = hasPermission(Permissions.Sales.Invoice.Create);
  const canEdit = hasPermission(Permissions.Sales.Invoice.Edit);
  const canDelete = hasPermission(Permissions.Sales.Invoice.Delete);

  const resetForm = () => { setForm(emptyForm); setError(null); };
  const openModal = () => { resetForm(); setShowModal(true); };
  const closeModal = () => { setShowModal(false); resetForm(); };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (form.customerId === "") { setError("Customer is required."); return; }
    try {
      const invoice = await createMutation.mutateAsync({
        customerId: Number(form.customerId),
        currencyId: null,
      });
      closeModal();
      navigate(`/sales/invoices/${invoice.id}`);
    } catch (err) {
      setError(extractErrorMessage(err));
    }
  };

  const confirmDelete = async () => {
    if (pendingDeleteId === null) return;
    try {
      await deleteMutation.mutateAsync(pendingDeleteId);
      setPendingDeleteId(null);
    } catch (_) {
      setPendingDeleteId(null);
    }
  };

  const columns = [
    { key: "invoiceNo", label: "Invoice No", sortable: true },
    { key: "customerName", label: "Customer", sortable: true },
    { key: "category", label: "Category", sortable: true },
    { key: "grandTotal", label: "Grand Total", sortable: true },
    { key: "dueAmount", label: "Due", sortable: true },
    { key: "paymentStatus", label: "Status", sortable: true },
    { key: "createdAt", label: "Date", sortable: true },
    ...(canDelete ? [{ key: "actions", label: "" }] : []),
  ];

  const renderRow = (inv: SalesInvoiceItem) => (
    <tr key={inv.id}>
      <td>
        {canEdit ? (
          <button type='button' className='btn btn-link p-0 fw-semibold' onClick={() => navigate(`/sales/invoices/${inv.id}`)}>
            {inv.invoiceNo}
          </button>
        ) : inv.invoiceNo}
      </td>
      <td>{inv.customerName}</td>
      <td><span className='badge bg-info bg-opacity-25 text-info'>{CATEGORY_LABELS[inv.category] ?? inv.category}</span></td>
      <td className='text-end'>{inv.grandTotal.toFixed(2)}</td>
      <td className='text-end'>{inv.dueAmount.toFixed(2)}</td>
      <td><span className={STATUS_BADGES[inv.paymentStatus] ?? "badge bg-secondary"}>{STATUS_LABELS[inv.paymentStatus] ?? inv.paymentStatus}</span></td>
      <td>{new Date(inv.createdAt).toLocaleDateString()}</td>
      {canDelete && (
        <td>
          <button type='button' className='btn btn-sm btn-outline-danger text-danger' onClick={() => setPendingDeleteId(inv.id)}>
            Delete
          </button>
        </td>
      )}
    </tr>
  );

  return (
    <div className='row gy-4'>
      {/* Category filter + create */}
      <div className='col-12'>
        <div className='d-flex align-items-center gap-2 flex-wrap'>
          {([null, 2, 1, 3, 4] as (SalesInvoiceCategory | null)[]).map(cat => (
            <button key={cat ?? "all"} type='button'
              className={`btn btn-sm ${categoryFilter === cat ? "btn-primary" : "btn-outline-primary"}`}
              onClick={() => setCategoryFilter(cat)}>
              {cat === null ? "All" : CATEGORY_LABELS[cat]}
            </button>
          ))}
          {canCreate && (
            <button type='button' className='btn btn-primary btn-sm ms-auto' onClick={openModal}>
              + New Draft
            </button>
          )}
        </div>
      </div>

      {/* Delete Confirm Modal */}
      <Modal show={pendingDeleteId !== null} onHide={() => setPendingDeleteId(null)} centered size="sm">
        <Modal.Body>This will delete the invoice and reverse all payments. Continue?</Modal.Body>
        <Modal.Footer>
          <button className="btn btn-danger btn-sm" disabled={deleteMutation.isPending} onClick={confirmDelete}>Delete</button>
          <button className="btn btn-outline-secondary btn-sm" onClick={() => setPendingDeleteId(null)}>Cancel</button>
        </Modal.Footer>
      </Modal>

      <div className='col-12'>
        <PaginatedTable
          title='Sales Invoices'
          columns={columns}
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
          renderRow={renderRow}
          emptyLabel='No invoices found.'
        />
      </div>

      {/* Create Draft Modal */}
      <Modal show={showModal} onHide={closeModal} centered>
        <form onSubmit={handleCreate}>
          <Modal.Header closeButton>
            <Modal.Title>New Draft Invoice</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            {error && <div className="alert alert-danger py-2">{error}</div>}
            <div className="mb-3">
              <label className="form-label">Customer *</label>
              <select
                className="form-select"
                value={form.customerId}
                onChange={e => setForm(f => ({ ...f, customerId: e.target.value === "" ? "" : Number(e.target.value) }))}
                required
              >
                <option value="">— select customer —</option>
                {customers.map(c => <option key={c.id} value={c.id}>{c.code} — {c.name}</option>)}
              </select>
            </div>
          </Modal.Body>
          <Modal.Footer>
            <button type="button" className="btn btn-outline-secondary" onClick={closeModal}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={createMutation.isPending}>
              {createMutation.isPending ? "Creating…" : "Create Draft"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>
    </div>
  );
};

export default SalesInvoicesLayer;
