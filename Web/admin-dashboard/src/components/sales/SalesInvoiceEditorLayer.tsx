import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useSalesInvoiceDetail, useSalesInvoices } from "../../hook/useSalesInvoices";
import { useSalesInvoiceLines } from "../../hook/useSalesInvoiceLines";
import { useSalesPayments } from "../../hook/useSalesPayments";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import { useQuery } from "@tanstack/react-query";
import { productsApi } from "../../api/settings/products";
import { accountsApi } from "../../api/accounts/accounts";
import { extractErrorMessage } from "../settings/extractErrorMessage";
import type { SalesInvoiceCategory } from "../../api/sales/salesInvoices";
import { LHDN_EINVOICE_TYPES } from "../../api/settings/lhdnEInvoiceTypes";
import { paymentCodesApi } from "../../api/settings/paymentCodes";

const CATEGORY_LABELS: Record<number, string> = { 1: "Regular", 2: "Draft", 3: "Quote", 4: "Manual" };
const STATUS_LABELS: Record<number, string> = { 0: "Unpaid", 1: "Partial", 2: "Paid" };

interface LineFormState {
  productId: number | "";
  quantity: string;
  unitPrice: string;
  itemVAT: string;
  itemDiscount: string;
}

interface PaymentFormState {
  accAccountId: number | "";
  modeOfPayment: string;
  amount: string;
  referenceNo: string;
}

const emptyLine: LineFormState = { productId: "", quantity: "", unitPrice: "", itemVAT: "0", itemDiscount: "0" };
const emptyPayment: PaymentFormState = { accAccountId: "", modeOfPayment: "", amount: "", referenceNo: "" };

const SalesInvoiceEditorLayer = () => {
  const { id } = useParams<{ id: string }>();
  const invoiceId = Number(id);
  const navigate = useNavigate();
  const { hasPermission } = usePermissions();

  const { data: invoice, isLoading } = useSalesInvoiceDetail(invoiceId);
  const { promoteMutation, updateMutation } = useSalesInvoices();
  const { linesQuery, addMutation: addLineMutation, removeMutation: removeLineMutation } = useSalesInvoiceLines(invoiceId);
  const { paymentsQuery, recordMutation, deleteMutation: deletePaymentMutation } = useSalesPayments(invoiceId);

  const productsQuery = useQuery({ queryKey: ["settings", "products", "all-active"], queryFn: productsApi.listAllActive });
  const accountsQuery = useQuery({ queryKey: ["accounts", "all-active"], queryFn: accountsApi.listAllActive });
  const paymentCodesQuery = useQuery({ queryKey: ["settings", "payment-codes", "all-active"], queryFn: paymentCodesApi.listAllActive });

  const canEdit = hasPermission(Permissions.Sales.Invoice.Edit);
  const canManage = hasPermission(Permissions.Sales.Invoice.Manage);

  const [lineForm, setLineForm] = useState<LineFormState>(emptyLine);
  const [paymentForm, setPaymentForm] = useState<PaymentFormState>(emptyPayment);
  const [lineError, setLineError] = useState<string | null>(null);
  const [paymentError, setPaymentError] = useState<string | null>(null);
  const [promoteError, setPromoteError] = useState<string | null>(null);
  const [typeCode, setTypeCode] = useState<string>("01");

  useEffect(() => {
    if (invoice?.invoiceTypeCode) setTypeCode(invoice.invoiceTypeCode);
  }, [invoice?.invoiceTypeCode]);

  const products = productsQuery.data ?? [];
  const accounts = accountsQuery.data ?? [];
  const lines = linesQuery.data ?? [];
  const payments = paymentsQuery.data ?? [];

  const handleProductChange = (productId: number | "") => {
    if (productId === "") { setLineForm(f => ({ ...f, productId: "" })); return; }
    const product = products.find(p => p.id === productId);
    setLineForm(f => ({
      ...f,
      productId,
      unitPrice: product ? String(product.unitSellingPrice ?? "") : "",
    }));
  };

  const handleAddLine = async (e: React.FormEvent) => {
    e.preventDefault();
    if (lineForm.productId === "") { setLineError("Product is required."); return; }
    try {
      await addLineMutation.mutateAsync({
        salesInvoiceId: invoiceId,
        productId: Number(lineForm.productId),
        quantity: Number(lineForm.quantity),
        unitPrice: Number(lineForm.unitPrice),
        itemVAT: Number(lineForm.itemVAT),
        itemDiscount: Number(lineForm.itemDiscount),
      });
      setLineForm(emptyLine);
      setLineError(null);
    } catch (err) {
      setLineError(extractErrorMessage(err));
    }
  };

  const handleRecordPayment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (paymentForm.accAccountId === "") { setPaymentError("Account is required."); return; }
    try {
      await recordMutation.mutateAsync({
        salesInvoiceId: invoiceId,
        accAccountId: Number(paymentForm.accAccountId),
        modeOfPayment: paymentForm.modeOfPayment,
        amount: Number(paymentForm.amount),
        referenceNo: paymentForm.referenceNo || null,
      });
      setPaymentForm(emptyPayment);
      setPaymentError(null);
    } catch (err) {
      setPaymentError(extractErrorMessage(err));
    }
  };

  const handlePromote = async (targetCategory: SalesInvoiceCategory) => {
    try {
      await promoteMutation.mutateAsync({ id: invoiceId, targetCategory });
      setPromoteError(null);
    } catch (err) {
      setPromoteError(extractErrorMessage(err));
    }
  };

  const handleTypeCodeChange = async (code: string) => {
    if (!invoice) return;
    setTypeCode(code);
    try {
      await updateMutation.mutateAsync({
        id: invoiceId,
        dto: {
          customerId: invoice.customerId,
          currencyId: invoice.currencyId,
          notes: invoice.notes ?? null,
          invoiceTypeCode: code,
        },
      });
    } catch {
      // revert on failure
      setTypeCode(invoice.invoiceTypeCode ?? "01");
    }
  };

  if (isLoading) return <div className='card p-24'><div className='spinner-border' /></div>;
  if (!invoice) return <div className='card p-24 text-danger'>Invoice not found.</div>;

  return (
    <div className='row gy-4'>
      {/* Invoice Header */}
      <div className='col-12'>
        <div className='card p-24'>
          <div className='d-flex align-items-center gap-3 flex-wrap'>
            <div>
              <h5 className='mb-1'>{invoice.invoiceNo}</h5>
              <div className='text-muted small'>{invoice.customerName}</div>
            </div>
            <span className='badge bg-info bg-opacity-25 text-info'>{CATEGORY_LABELS[invoice.category]}</span>
            <span className='badge bg-secondary-focus text-secondary-600'>{STATUS_LABELS[invoice.paymentStatus]}</span>
            {canEdit ? (
              <select
                className='form-select form-select-sm'
                style={{ width: 'auto' }}
                value={typeCode}
                onChange={e => handleTypeCodeChange(e.target.value)}
                disabled={updateMutation.isPending}
              >
                {LHDN_EINVOICE_TYPES.map(t => (
                  <option key={t.code} value={t.code}>{t.code} — {t.description}</option>
                ))}
              </select>
            ) : (
              <span className='badge bg-light text-dark fw-semibold'>
                {typeCode} — {LHDN_EINVOICE_TYPES.find(t => t.code === typeCode)?.description ?? typeCode}
              </span>
            )}
            <div className='ms-auto text-end d-flex flex-column align-items-end gap-1'>
              <div>
                <div className='text-muted small'>Grand Total</div>
                <div className='fs-5 fw-bold'>{invoice.grandTotal.toFixed(2)}</div>
                {invoice.dueAmount > 0 && (
                  <div className='text-danger small'>Due: {invoice.dueAmount.toFixed(2)}</div>
                )}
              </div>
              <button
                type='button'
                className='btn btn-sm btn-outline-secondary'
                onClick={() => navigate(`/sales/invoices/${invoiceId}/view`)}
              >
                View / Print
              </button>
            </div>
          </div>

          {/* Promote actions (only for Draft) */}
          {invoice.category === 2 && canManage && (
            <div className='mt-3 d-flex gap-2 flex-wrap'>
              <span className='text-muted small'>Promote to:</span>
              {([1, 3, 4] as SalesInvoiceCategory[]).map(cat => (
                <button key={cat} type='button' className='btn btn-sm btn-outline-primary'
                  onClick={() => handlePromote(cat)} disabled={promoteMutation.isPending}>
                  {CATEGORY_LABELS[cat]}
                </button>
              ))}
              {promoteError && <span className='text-danger small'>{promoteError}</span>}
            </div>
          )}
        </div>
      </div>

      {/* Invoice Lines */}
      <div className='col-12'>
        <div className='card p-24'>
          <h6 className='mb-3'>Lines</h6>

          {canEdit && (
            <form onSubmit={handleAddLine} className='mb-3'>
              {lineError && <div className='alert alert-danger py-2 mb-2'>{lineError}</div>}
              <div className='row g-2'>
                <div className='col-md-4'>
                  <select className='form-select form-select-sm' value={lineForm.productId}
                    onChange={e => handleProductChange(e.target.value === "" ? "" : Number(e.target.value))} required>
                    <option value=''>— Product —</option>
                    {products.map(p => <option key={p.id} value={p.id}>{p.code} — {p.description}</option>)}
                  </select>
                </div>
                <div className='col-md-2'>
                  <input type='number' className='form-control form-control-sm' placeholder='Qty' min='0.001' step='any'
                    value={lineForm.quantity} onChange={e => setLineForm(f => ({ ...f, quantity: e.target.value }))} required />
                </div>
                <div className='col-md-2'>
                  <input type='number' className='form-control form-control-sm' placeholder='Unit Price' min='0' step='any'
                    value={lineForm.unitPrice} onChange={e => setLineForm(f => ({ ...f, unitPrice: e.target.value }))} required />
                </div>
                <div className='col-md-1'>
                  <input type='number' className='form-control form-control-sm' placeholder='VAT %' min='0' step='any'
                    value={lineForm.itemVAT} onChange={e => setLineForm(f => ({ ...f, itemVAT: e.target.value }))} />
                </div>
                <div className='col-md-1'>
                  <input type='number' className='form-control form-control-sm' placeholder='Disc %' min='0' step='any'
                    value={lineForm.itemDiscount} onChange={e => setLineForm(f => ({ ...f, itemDiscount: e.target.value }))} />
                </div>
                <div className='col-md-2'>
                  <button type='submit' className='btn btn-primary btn-sm w-100' disabled={addLineMutation.isPending}>
                    {addLineMutation.isPending ? "Adding…" : "Add Line"}
                  </button>
                </div>
              </div>
            </form>
          )}

          <table className='table table-sm'>
            <thead>
              <tr>
                <th>Product</th>
                <th className='text-end'>Qty</th>
                <th className='text-end'>Price</th>
                <th className='text-end'>Disc%</th>
                <th className='text-end'>VAT%</th>
                <th className='text-end'>Total</th>
                <th>LHDN</th>
                {canEdit && <th />}
              </tr>
            </thead>
            <tbody>
              {linesQuery.isLoading ? (
                <tr><td colSpan={8} className='text-center'><span className='spinner-border spinner-border-sm' /></td></tr>
              ) : lines.length === 0 ? (
                <tr><td colSpan={8} className='text-center text-muted'>No lines yet.</td></tr>
              ) : lines.map(line => (
                <tr key={line.id}>
                  <td>{line.itemName}</td>
                  <td className='text-end'>{line.quantity}</td>
                  <td className='text-end'>{line.unitPrice.toFixed(2)}</td>
                  <td className='text-end'>{line.itemDiscount}%</td>
                  <td className='text-end'>{line.itemVat}%</td>
                  <td className='text-end'>{line.totalAmount.toFixed(2)}</td>
                  <td>
                    <div className='d-flex flex-wrap gap-1'>
                      {line.classificationCode && <span className='badge bg-info bg-opacity-15 text-info border border-info border-opacity-25' style={{fontSize:10}}>{line.classificationCode}</span>}
                      {line.taxTypeCode && <span className='badge bg-success bg-opacity-15 text-success border border-success border-opacity-25' style={{fontSize:10}}>{line.taxTypeCode}</span>}
                      {line.unitCode && <span className='badge bg-secondary bg-opacity-15 text-secondary border border-secondary border-opacity-25' style={{fontSize:10}}>{line.unitCode}</span>}
                    </div>
                  </td>
                  {canEdit && (
                    <td>
                      <button type='button' className='btn btn-sm btn-outline-danger text-danger'
                        onClick={() => removeLineMutation.mutate(line.id)} disabled={removeLineMutation.isPending}>
                        Remove
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
            {lines.length > 0 && (
              <tfoot>
                <tr className='fw-bold'>
                  <td colSpan={5} className='text-end'>Grand Total</td>
                  <td className='text-end'>{invoice.grandTotal.toFixed(2)}</td>
                  <td />
                  {canEdit && <td />}
                </tr>
              </tfoot>
            )}
          </table>
        </div>
      </div>

      {/* Payments */}
      <div className='col-12'>
        <div className='card p-24'>
          <h6 className='mb-3'>Payments</h6>

          {canManage && (
            <form onSubmit={handleRecordPayment} className='mb-3'>
              {paymentError && <div className='alert alert-danger py-2 mb-2'>{paymentError}</div>}
              <div className='row g-2'>
                <div className='col-md-3'>
                  <select className='form-select form-select-sm' value={paymentForm.accAccountId}
                    onChange={e => setPaymentForm(f => ({ ...f, accAccountId: e.target.value === "" ? "" : Number(e.target.value) }))} required>
                    <option value=''>— Account —</option>
                    {accounts.map(a => <option key={a.id} value={a.id}>{a.accountName}</option>)}
                  </select>
                </div>
                <div className='col-md-2'>
                  <select className='form-select form-select-sm' value={paymentForm.modeOfPayment}
                    onChange={e => setPaymentForm(f => ({ ...f, modeOfPayment: e.target.value }))}>
                    <option value=''>— Select —</option>
                    {(paymentCodesQuery.data ?? []).map(m => (
                      <option key={m.code} value={m.code}>{m.code} — {m.name}</option>
                    ))}
                  </select>
                </div>
                <div className='col-md-2'>
                  <input type='number' className='form-control form-control-sm' placeholder='Amount' min='0.01' step='any'
                    value={paymentForm.amount} onChange={e => setPaymentForm(f => ({ ...f, amount: e.target.value }))} required />
                </div>
                <div className='col-md-3'>
                  <input type='text' className='form-control form-control-sm' placeholder='Reference No (optional)'
                    value={paymentForm.referenceNo} onChange={e => setPaymentForm(f => ({ ...f, referenceNo: e.target.value }))} />
                </div>
                <div className='col-md-2'>
                  <button type='submit' className='btn btn-primary btn-sm w-100' disabled={recordMutation.isPending}>
                    {recordMutation.isPending ? "Recording…" : "Record Payment"}
                  </button>
                </div>
              </div>
            </form>
          )}

          <table className='table table-sm'>
            <thead>
              <tr>
                <th>Account</th>
                <th>Mode</th>
                <th className='text-end'>Amount</th>
                <th>Reference</th>
                {canManage && <th />}
              </tr>
            </thead>
            <tbody>
              {paymentsQuery.isLoading ? (
                <tr><td colSpan={5} className='text-center'><span className='spinner-border spinner-border-sm' /></td></tr>
              ) : payments.length === 0 ? (
                <tr><td colSpan={5} className='text-center text-muted'>No payments yet.</td></tr>
              ) : payments.map(pmt => (
                <tr key={pmt.id}>
                  <td>{pmt.accAccountName}</td>
                  <td>{pmt.modeOfPayment}</td>
                  <td className='text-end'>{pmt.amount.toFixed(2)}</td>
                  <td>{pmt.referenceNo ?? "—"}</td>
                  {canManage && (
                    <td>
                      <button type='button' className='btn btn-sm btn-outline-danger text-danger'
                        onClick={() => deletePaymentMutation.mutate(pmt.id)} disabled={deletePaymentMutation.isPending}>
                        Reverse
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default SalesInvoiceEditorLayer;
