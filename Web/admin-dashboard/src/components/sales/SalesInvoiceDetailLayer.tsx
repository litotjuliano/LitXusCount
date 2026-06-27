import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { salesInvoicesApi } from "../../api/sales/salesInvoices";
import { salesInvoiceLinesApi } from "../../api/sales/salesInvoiceLines";
import { salesPaymentsApi } from "../../api/sales/salesPayments";
import { companyInfoApi } from "../../api/settings/companyInfo";
import { customersApi } from "../../api/settings/customers";
import { LHDN_EINVOICE_TYPES } from "../../api/settings/lhdnEInvoiceTypes";

const CATEGORY_LABELS: Record<number, string> = { 1: "TAX INVOICE", 2: "DRAFT", 3: "QUOTATION", 4: "INVOICE" };
const STATUS_LABELS: Record<number, string> = { 0: "Unpaid", 1: "Partially Paid", 2: "Paid" };
const STATUS_COLORS: Record<number, string> = { 0: "#dc2626", 1: "#d97706", 2: "#16a34a" };

const fmt = (n: number) => n.toLocaleString("en-MY", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const SalesInvoiceDetailLayer = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const invoiceId = Number(id);

  const invoiceQ  = useQuery({ queryKey: ["sales", "invoices", invoiceId],          queryFn: () => salesInvoicesApi.getById(invoiceId) });
  const linesQ    = useQuery({ queryKey: ["sales", "invoice-lines", invoiceId],     queryFn: () => salesInvoiceLinesApi.getByInvoice(invoiceId) });
  const paymentsQ = useQuery({ queryKey: ["sales", "payments", invoiceId],          queryFn: () => salesPaymentsApi.getByInvoice(invoiceId) });
  const companyQ  = useQuery({ queryKey: ["settings", "company-info"],              queryFn: companyInfoApi.get });
  const customersQ = useQuery({ queryKey: ["settings", "customers", "all-active"],  queryFn: customersApi.listAllActive });

  if (invoiceQ.isLoading || companyQ.isLoading) {
    return <div className="d-flex justify-content-center py-5"><div className="spinner-border" /></div>;
  }
  if (!invoiceQ.data) return <div className="alert alert-danger">Invoice not found.</div>;

  const inv     = invoiceQ.data;
  const lines   = linesQ.data ?? [];
  const pmts    = paymentsQ.data ?? [];
  const co      = companyQ.data;
  const customer = customersQ.data?.find(c => c.id === inv.customerId);
  const invType  = LHDN_EINVOICE_TYPES.find(t => t.code === inv.invoiceTypeCode);
  const invoiceDate = new Date(inv.createdAt).toLocaleDateString("en-MY", { day: "2-digit", month: "long", year: "numeric" });

  return (
    <>
      {/* Screen-only toolbar */}
      <div className="d-print-none d-flex gap-2 mb-3">
        <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate(-1)}>
          ← Back
        </button>
        <button className="btn btn-primary btn-sm ms-auto" onClick={() => window.print()}>
          Print / Save PDF
        </button>
      </div>

      {/* Invoice document */}
      <div className="card" id="invoice-print-area" style={{ maxWidth: 900, margin: "0 auto", fontFamily: "Arial, sans-serif", fontSize: 13 }}>
        <div className="card-body p-4">

          {/* ── Header ─────────────────────────────────────────── */}
          <div className="row mb-4 align-items-start">
            <div className="col-7">
              {co?.logoUrl && (
                <img src={co.logoUrl} alt="logo" style={{ maxHeight: 60, maxWidth: 160, objectFit: "contain", marginBottom: 8 }} />
              )}
              <div style={{ fontWeight: 700, fontSize: 16 }}>{co?.name ?? "—"}</div>
              {co?.address && <div className="text-muted" style={{ fontSize: 12 }}>{co.address}</div>}
              {(co?.city || co?.postCode) && (
                <div className="text-muted" style={{ fontSize: 12 }}>
                  {[co.postCode, co.city, co.country].filter(Boolean).join(", ")}
                </div>
              )}
              {co?.phone && <div className="text-muted" style={{ fontSize: 12 }}>Tel: {co.phone}</div>}
              {co?.email && <div className="text-muted" style={{ fontSize: 12 }}>Email: {co.email}</div>}
              {(co?.vatRegistrationNumber || co?.sSTRegistrationNumber) && (
                <div style={{ fontSize: 12, marginTop: 4 }}>
                  <strong>SST Reg:</strong> {co.sSTRegistrationNumber ?? co.vatRegistrationNumber}
                </div>
              )}
              {co?.tin && (
                <div style={{ fontSize: 12 }}>
                  <strong>TIN:</strong> {co.tin}
                </div>
              )}
            </div>
            <div className="col-5 text-end">
              <div style={{ fontSize: 22, fontWeight: 700, color: "#1e3a5f", letterSpacing: 1 }}>
                {CATEGORY_LABELS[inv.category] ?? "INVOICE"}
              </div>
              {invType && (
                <div className="text-muted" style={{ fontSize: 11 }}>{invType.code} — {invType.description}</div>
              )}
              <table className="ms-auto mt-2" style={{ fontSize: 13 }}>
                <tbody>
                  <tr>
                    <td className="text-muted pe-3">Invoice No</td>
                    <td style={{ fontWeight: 600 }}>{inv.invoiceNo}</td>
                  </tr>
                  <tr>
                    <td className="text-muted pe-3">Date</td>
                    <td>{invoiceDate}</td>
                  </tr>
                  <tr>
                    <td className="text-muted pe-3">Currency</td>
                    <td>{inv.currencyCode ?? "MYR"}</td>
                  </tr>
                  <tr>
                    <td className="text-muted pe-3">Status</td>
                    <td style={{ color: STATUS_COLORS[inv.paymentStatus], fontWeight: 600 }}>
                      {STATUS_LABELS[inv.paymentStatus]}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <hr />

          {/* ── Bill To ────────────────────────────────────────── */}
          <div className="row mb-4">
            <div className="col-6">
              <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", color: "#6b7280", marginBottom: 4 }}>Bill To</div>
              <div style={{ fontWeight: 600, fontSize: 14 }}>{inv.customerName}</div>
              {customer && (
                <>
                  {customer.address1 && <div style={{ fontSize: 12 }}>{customer.address1}</div>}
                  {customer.address2 && <div style={{ fontSize: 12 }}>{customer.address2}</div>}
                  {(customer.addressCode || customer.city) && (
                    <div style={{ fontSize: 12 }}>
                      {[customer.addressCode, customer.city, customer.state, customer.country].filter(Boolean).join(", ")}
                    </div>
                  )}
                  {customer.phone && <div style={{ fontSize: 12 }}>Tel: {customer.phone}</div>}
                  {customer.email && <div style={{ fontSize: 12 }}>Email: {customer.email}</div>}
                  {customer.tin && <div style={{ fontSize: 12 }}><strong>TIN:</strong> {customer.tin}</div>}
                  {customer.registrationNumber && (
                    <div style={{ fontSize: 12 }}>
                      <strong>{customer.registrationType ?? "Reg"}:</strong> {customer.registrationNumber}
                    </div>
                  )}
                  {customer.sSTRegistrationNumber && (
                    <div style={{ fontSize: 12 }}><strong>SST:</strong> {customer.sSTRegistrationNumber}</div>
                  )}
                </>
              )}
            </div>
            {inv.notes && (
              <div className="col-6">
                <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", color: "#6b7280", marginBottom: 4 }}>Notes</div>
                <div style={{ fontSize: 12, color: "#374151" }}>{inv.notes}</div>
              </div>
            )}
          </div>

          {/* ── Line Items ─────────────────────────────────────── */}
          <table className="table table-bordered table-sm mb-0" style={{ fontSize: 12 }}>
            <thead style={{ backgroundColor: "#1e3a5f", color: "#fff" }}>
              <tr>
                <th style={{ width: 30 }}>#</th>
                <th>Description</th>
                <th className="text-end" style={{ width: 60 }}>Qty</th>
                <th className="text-end" style={{ width: 90 }}>Unit Price</th>
                <th className="text-end" style={{ width: 60 }}>Disc%</th>
                <th className="text-end" style={{ width: 60 }}>SST%</th>
                <th className="text-end" style={{ width: 90 }}>SST Amt</th>
                <th className="text-end" style={{ width: 90 }}>Total</th>
              </tr>
            </thead>
            <tbody>
              {lines.length === 0 ? (
                <tr><td colSpan={8} className="text-center text-muted py-3">No line items.</td></tr>
              ) : lines.map((line, idx) => (
                <tr key={line.id} style={line.isReturn ? { backgroundColor: "#fef2f2" } : undefined}>
                  <td className="text-muted">{idx + 1}</td>
                  <td>
                    {line.itemName}
                    {line.isReturn && <span className="badge bg-danger ms-1" style={{ fontSize: 9 }}>RETURN</span>}
                    {(line.classificationCode || line.taxTypeCode || line.unitCode) && (
                      <div className="d-flex gap-1 mt-1">
                        {line.classificationCode && (
                          <span style={{ fontSize: 9, padding: "1px 5px", borderRadius: 3, background: "#dbeafe", color: "#1d4ed8" }}>{line.classificationCode}</span>
                        )}
                        {line.taxTypeCode && (
                          <span style={{ fontSize: 9, padding: "1px 5px", borderRadius: 3, background: "#dcfce7", color: "#15803d" }}>{line.taxTypeCode}</span>
                        )}
                        {line.unitCode && (
                          <span style={{ fontSize: 9, padding: "1px 5px", borderRadius: 3, background: "#f3f4f6", color: "#374151" }}>{line.unitCode}</span>
                        )}
                      </div>
                    )}
                  </td>
                  <td className="text-end">{line.quantity}</td>
                  <td className="text-end">{fmt(line.unitPrice)}</td>
                  <td className="text-end">{line.itemDiscount > 0 ? `${line.itemDiscount}%` : "—"}</td>
                  <td className="text-end">{line.itemVat > 0 ? `${line.itemVat}%` : "—"}</td>
                  <td className="text-end">{line.itemVatAmount > 0 ? fmt(line.itemVatAmount) : "—"}</td>
                  <td className="text-end" style={{ fontWeight: 600 }}>{fmt(line.totalAmount)}</td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* ── Totals ─────────────────────────────────────────── */}
          <div className="row mt-3">
            <div className="col-6" />
            <div className="col-6">
              <table className="w-100" style={{ fontSize: 13 }}>
                <tbody>
                  <tr>
                    <td className="text-muted py-1">Subtotal</td>
                    <td className="text-end py-1">{fmt(inv.subTotal)}</td>
                  </tr>
                  {inv.discountAmount > 0 && (
                    <tr>
                      <td className="text-muted py-1">Discount</td>
                      <td className="text-end py-1 text-danger">({fmt(inv.discountAmount)})</td>
                    </tr>
                  )}
                  {inv.vatAmount > 0 && (
                    <tr>
                      <td className="text-muted py-1">SST Amount</td>
                      <td className="text-end py-1">{fmt(inv.vatAmount)}</td>
                    </tr>
                  )}
                  <tr style={{ borderTop: "2px solid #1e3a5f" }}>
                    <td className="py-2" style={{ fontWeight: 700, fontSize: 14 }}>Grand Total ({inv.currencyCode ?? "MYR"})</td>
                    <td className="text-end py-2" style={{ fontWeight: 700, fontSize: 14 }}>{fmt(inv.grandTotal)}</td>
                  </tr>
                  {inv.paidAmount > 0 && (
                    <tr>
                      <td className="text-muted py-1">Paid</td>
                      <td className="text-end py-1 text-success">{fmt(inv.paidAmount)}</td>
                    </tr>
                  )}
                  {inv.dueAmount > 0 && (
                    <tr style={{ borderTop: "1px solid #e5e7eb" }}>
                      <td className="py-1" style={{ fontWeight: 600, color: "#dc2626" }}>Balance Due</td>
                      <td className="text-end py-1" style={{ fontWeight: 600, color: "#dc2626" }}>{fmt(inv.dueAmount)}</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          {/* ── Payments ───────────────────────────────────────── */}
          {pmts.length > 0 && (
            <div className="mt-4">
              <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", color: "#6b7280", marginBottom: 6 }}>Payment Records</div>
              <table className="table table-sm table-bordered" style={{ fontSize: 12 }}>
                <thead style={{ backgroundColor: "#f9fafb" }}>
                  <tr>
                    <th>Account</th>
                    <th>Mode</th>
                    <th>Reference</th>
                    <th className="text-end">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  {pmts.map(p => (
                    <tr key={p.id}>
                      <td>{p.accAccountName}</td>
                      <td>{p.modeOfPayment || "—"}</td>
                      <td>{p.referenceNo ?? "—"}</td>
                      <td className="text-end">{fmt(p.amount)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {/* ── Terms ──────────────────────────────────────────── */}
          {co?.termsAndConditions && (
            <div className="mt-4 pt-3" style={{ borderTop: "1px solid #e5e7eb" }}>
              <div style={{ fontSize: 11, fontWeight: 700, textTransform: "uppercase", color: "#6b7280", marginBottom: 4 }}>Terms &amp; Conditions</div>
              <div style={{ fontSize: 11, color: "#6b7280", lineHeight: 1.5 }}>{co.termsAndConditions}</div>
            </div>
          )}

          {/* ── Footer ─────────────────────────────────────────── */}
          <div className="mt-4 text-center text-muted" style={{ fontSize: 10, borderTop: "1px solid #e5e7eb", paddingTop: 8 }}>
            This is a computer-generated document. No signature is required.
            {co?.website && <> | {co.website}</>}
          </div>

        </div>
      </div>

      {/* Print styles */}
      <style>{`
        @media print {
          body * { visibility: hidden; }
          #invoice-print-area, #invoice-print-area * { visibility: visible; }
          #invoice-print-area { position: absolute; left: 0; top: 0; width: 100%; box-shadow: none !important; border: none !important; }
          .d-print-none { display: none !important; }
        }
      `}</style>
    </>
  );
};

export default SalesInvoiceDetailLayer;
