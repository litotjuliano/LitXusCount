import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useSupplierSettings } from "../../hook/useSupplierSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { SupplierItem } from "../../api/settings/suppliers";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  code: string; name: string; glAccountId: number | "";
  address1: string; address2: string; address3: string; addressCode: string;
  city: string; state: string; country: string;
  phone: string; fax: string; email: string; contactPerson: string;
  paymentTermsDays: number; defaultCurrencyId: number | "";
}

const emptyForm: FormState = {
  code: "", name: "", glAccountId: "",
  address1: "", address2: "", address3: "", addressCode: "",
  city: "", state: "", country: "",
  phone: "", fax: "", email: "", contactPerson: "",
  paymentTermsDays: 0, defaultCurrencyId: "",
};

const SupplierSettingsLayer = () => {
  const { pagedQuery, glAccountsQuery, currenciesQuery, createMutation, editMutation, deleteMutation } = useSupplierSettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Settings.Supplier.Create);
  const canEdit = hasPermission(Permissions.Settings.Supplier.Edit);
  const canDelete = hasPermission(Permissions.Settings.Supplier.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;
  const glAccounts = glAccountsQuery.data ?? [];
  const currencies = currenciesQuery.data ?? [];
  const n = (v: string) => v || null;
  const f = (k: keyof FormState, v: string) => setForm((prev) => ({ ...prev, [k]: v }));

  const openAdd = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(true); };

  const startEdit = (item: SupplierItem) => {
    setEditingId(item.id);
    setForm({
      code: item.code, name: item.name, glAccountId: item.glAccountId,
      address1: item.address1 ?? "", address2: item.address2 ?? "", address3: item.address3 ?? "",
      addressCode: item.addressCode ?? "", city: item.city ?? "", state: item.state ?? "", country: item.country ?? "",
      phone: item.phone ?? "", fax: item.fax ?? "", email: item.email ?? "", contactPerson: item.contactPerson ?? "",
      paymentTermsDays: item.paymentTermsDays, defaultCurrencyId: item.defaultCurrencyId ?? "",
    });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(false); };

  const buildPayload = () => ({
    code: form.code, name: form.name, glAccountId: Number(form.glAccountId),
    address1: n(form.address1), address2: n(form.address2), address3: n(form.address3),
    addressCode: n(form.addressCode), city: n(form.city), state: n(form.state), country: n(form.country),
    phone: n(form.phone), fax: n(form.fax), email: n(form.email), contactPerson: n(form.contactPerson),
    paymentTermsDays: form.paymentTermsDays,
    defaultCurrencyId: form.defaultCurrencyId === "" ? null : Number(form.defaultCurrencyId),
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
      <Modal show={showModal} onHide={closeModal} centered size='xl' scrollable>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add Supplier" : "Edit Supplier"}</Modal.Title>
        </Modal.Header>
        <form id='supplier-form' onSubmit={handleSubmit}>
          <Modal.Body>
            <div className='row gy-3'>
              <div className='col-md-2'><label className='form-label'>Code <span className='text-danger'>*</span></label><input type='text' className='form-control' value={form.code} onChange={(e) => f("code", e.target.value)} required autoFocus /></div>
              <div className='col-md-4'><label className='form-label'>Name <span className='text-danger'>*</span></label><input type='text' className='form-control' value={form.name} onChange={(e) => f("name", e.target.value)} required /></div>
              <div className='col-md-3'>
                <label className='form-label'>GL Account <span className='text-danger'>*</span></label>
                <select className='form-select' value={form.glAccountId} onChange={(e) => setForm({ ...form, glAccountId: Number(e.target.value) })} required>
                  <option value=''>— Select —</option>
                  {glAccounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.name}</option>)}
                </select>
              </div>
              <div className='col-md-3'>
                <label className='form-label'>Default Currency</label>
                <select className='form-select' value={form.defaultCurrencyId} onChange={(e) => setForm({ ...form, defaultCurrencyId: e.target.value === "" ? "" : Number(e.target.value) })}>
                  <option value=''>— None —</option>
                  {currencies.map(c => <option key={c.id} value={c.id}>{c.code} — {c.name}</option>)}
                </select>
              </div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Address</p></div>
              <div className='col-md-4'><label className='form-label'>Address 1</label><input type='text' className='form-control' value={form.address1} onChange={(e) => f("address1", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Address 2</label><input type='text' className='form-control' value={form.address2} onChange={(e) => f("address2", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Address 3</label><input type='text' className='form-control' value={form.address3} onChange={(e) => f("address3", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Postal Code</label><input type='text' className='form-control' value={form.addressCode} onChange={(e) => f("addressCode", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>City</label><input type='text' className='form-control' value={form.city} onChange={(e) => f("city", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>State</label><input type='text' className='form-control' value={form.state} onChange={(e) => f("state", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Country</label><input type='text' className='form-control' value={form.country} onChange={(e) => f("country", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Phone</label><input type='text' className='form-control' value={form.phone} onChange={(e) => f("phone", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Fax</label><input type='text' className='form-control' value={form.fax} onChange={(e) => f("fax", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Email</label><input type='email' className='form-control' value={form.email} onChange={(e) => f("email", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Contact Person</label><input type='text' className='form-control' value={form.contactPerson} onChange={(e) => f("contactPerson", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Payment Terms (days)</label><input type='number' className='form-control' value={form.paymentTermsDays} onChange={(e) => setForm({ ...form, paymentTermsDays: parseInt(e.target.value) || 0 })} /></div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </Modal.Body>
          <Modal.Footer>
            <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
            <button type='submit' form='supplier-form' className='btn btn-primary' disabled={isSaving}>
              {editingId === null ? "Add" : "Save"}
            </button>
          </Modal.Footer>
        </form>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Suppliers'
            columns={[
              { key: "code", label: "Code", sortable: true },
              { key: "name", label: "Name", sortable: true },
              { key: "glAccount", label: "GL Account" },
              { key: "phone", label: "Phone" },
              { key: "email", label: "Email" },
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
                <td>{item.glAccountCode} — {item.glAccountName}</td>
                <td>{item.phone ?? "—"}</td>
                <td>{item.email ?? "—"}</td>
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

export default SupplierSettingsLayer;
