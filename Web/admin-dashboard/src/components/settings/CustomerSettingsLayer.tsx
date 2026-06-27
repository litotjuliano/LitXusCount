import { useState, type FormEvent } from "react";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useCustomerSettings } from "../../hook/useCustomerSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { CustomerItem } from "../../api/settings/customers";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  code: string; name: string; name2: string; glAccountId: number | "";
  address1: string; address2: string; address3: string; addressCode: string;
  city: string; state: string; country: string;
  phone: string; phone2: string; fax: string; email: string; contactPerson: string;
  consigneeName: string; consigneeAddress1: string; consigneeAddress2: string; consigneeAddress3: string;
  consigneeAddressCode: string; consigneeCity: string; consigneeState: string; consigneeCountry: string; consigneePhone: string;
  paymentTermsDays: number; creditLimit: number; isLocked: boolean;
}

const emptyForm: FormState = {
  code: "", name: "", name2: "", glAccountId: "",
  address1: "", address2: "", address3: "", addressCode: "", city: "", state: "", country: "",
  phone: "", phone2: "", fax: "", email: "", contactPerson: "",
  consigneeName: "", consigneeAddress1: "", consigneeAddress2: "", consigneeAddress3: "",
  consigneeAddressCode: "", consigneeCity: "", consigneeState: "", consigneeCountry: "", consigneePhone: "",
  paymentTermsDays: 0, creditLimit: 0, isLocked: false,
};

const CustomerSettingsLayer = () => {
  const { pagedQuery, glAccountsQuery, createMutation, editMutation, deleteMutation } = useCustomerSettings();
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Settings.Customer.Create);
  const canEdit = hasPermission(Permissions.Settings.Customer.Edit);
  const canDelete = hasPermission(Permissions.Settings.Customer.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;
  const glAccounts = glAccountsQuery.data ?? [];
  const n = (v: string) => v || null;
  const f = (k: keyof FormState, v: string) => setForm((prev) => ({ ...prev, [k]: v }));

  const openAdd = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(true); };

  const startEdit = (item: CustomerItem) => {
    setEditingId(item.id);
    setForm({
      code: item.code, name: item.name, name2: item.name2 ?? "", glAccountId: item.glAccountId,
      address1: item.address1 ?? "", address2: item.address2 ?? "", address3: item.address3 ?? "",
      addressCode: item.addressCode ?? "", city: item.city ?? "", state: item.state ?? "", country: item.country ?? "",
      phone: item.phone ?? "", phone2: item.phone2 ?? "", fax: item.fax ?? "", email: item.email ?? "", contactPerson: item.contactPerson ?? "",
      consigneeName: item.consigneeName ?? "", consigneeAddress1: item.consigneeAddress1 ?? "",
      consigneeAddress2: item.consigneeAddress2 ?? "", consigneeAddress3: item.consigneeAddress3 ?? "",
      consigneeAddressCode: item.consigneeAddressCode ?? "", consigneeCity: item.consigneeCity ?? "",
      consigneeState: item.consigneeState ?? "", consigneeCountry: item.consigneeCountry ?? "", consigneePhone: item.consigneePhone ?? "",
      paymentTermsDays: item.paymentTermsDays, creditLimit: item.creditLimit, isLocked: item.isLocked,
    });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(false); };

  const buildPayload = () => ({
    code: form.code, name: form.name, name2: n(form.name2), glAccountId: Number(form.glAccountId),
    address1: n(form.address1), address2: n(form.address2), address3: n(form.address3),
    addressCode: n(form.addressCode), city: n(form.city), state: n(form.state), country: n(form.country),
    phone: n(form.phone), phone2: n(form.phone2), fax: n(form.fax), email: n(form.email), contactPerson: n(form.contactPerson),
    consigneeName: n(form.consigneeName), consigneeAddress1: n(form.consigneeAddress1),
    consigneeAddress2: n(form.consigneeAddress2), consigneeAddress3: n(form.consigneeAddress3),
    consigneeAddressCode: n(form.consigneeAddressCode), consigneeCity: n(form.consigneeCity),
    consigneeState: n(form.consigneeState), consigneeCountry: n(form.consigneeCountry), consigneePhone: n(form.consigneePhone),
    paymentTermsDays: form.paymentTermsDays, creditLimit: form.creditLimit, isLocked: form.isLocked,
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
          <Modal.Title className='h6'>{editingId === null ? "Add Customer" : "Edit Customer"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form id='customer-form' onSubmit={handleSubmit}>
            <div className='row gy-3'>
              <div className='col-md-2'><label className='form-label'>Code <span className='text-danger'>*</span></label><input type='text' className='form-control' value={form.code} onChange={(e) => f("code", e.target.value)} required autoFocus /></div>
              <div className='col-md-4'><label className='form-label'>Name <span className='text-danger'>*</span></label><input type='text' className='form-control' value={form.name} onChange={(e) => f("name", e.target.value)} required /></div>
              <div className='col-md-3'><label className='form-label'>Name 2</label><input type='text' className='form-control' value={form.name2} onChange={(e) => f("name2", e.target.value)} /></div>
              <div className='col-md-3'>
                <label className='form-label'>GL Account <span className='text-danger'>*</span></label>
                <select className='form-select' value={form.glAccountId} onChange={(e) => setForm({ ...form, glAccountId: Number(e.target.value) })} required>
                  <option value=''>— Select —</option>
                  {glAccounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.name}</option>)}
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
              <div className='col-md-2'><label className='form-label'>Phone 2</label><input type='text' className='form-control' value={form.phone2} onChange={(e) => f("phone2", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Fax</label><input type='text' className='form-control' value={form.fax} onChange={(e) => f("fax", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Email</label><input type='email' className='form-control' value={form.email} onChange={(e) => f("email", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Contact Person</label><input type='text' className='form-control' value={form.contactPerson} onChange={(e) => f("contactPerson", e.target.value)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Consignee</p></div>
              <div className='col-md-4'><label className='form-label'>Consignee Name</label><input type='text' className='form-control' value={form.consigneeName} onChange={(e) => f("consigneeName", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Address 1</label><input type='text' className='form-control' value={form.consigneeAddress1} onChange={(e) => f("consigneeAddress1", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Address 2</label><input type='text' className='form-control' value={form.consigneeAddress2} onChange={(e) => f("consigneeAddress2", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Address 3</label><input type='text' className='form-control' value={form.consigneeAddress3} onChange={(e) => f("consigneeAddress3", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Postal Code</label><input type='text' className='form-control' value={form.consigneeAddressCode} onChange={(e) => f("consigneeAddressCode", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>City</label><input type='text' className='form-control' value={form.consigneeCity} onChange={(e) => f("consigneeCity", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>State</label><input type='text' className='form-control' value={form.consigneeState} onChange={(e) => f("consigneeState", e.target.value)} /></div>
              <div className='col-md-2'><label className='form-label'>Country</label><input type='text' className='form-control' value={form.consigneeCountry} onChange={(e) => f("consigneeCountry", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Phone</label><input type='text' className='form-control' value={form.consigneePhone} onChange={(e) => f("consigneePhone", e.target.value)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Terms</p></div>
              <div className='col-md-3'><label className='form-label'>Payment Terms (days)</label><input type='number' className='form-control' value={form.paymentTermsDays} onChange={(e) => setForm({ ...form, paymentTermsDays: parseInt(e.target.value) || 0 })} /></div>
              <div className='col-md-3'><label className='form-label'>Credit Limit</label><input type='number' className='form-control' step='0.01' value={form.creditLimit} onChange={(e) => setForm({ ...form, creditLimit: parseFloat(e.target.value) || 0 })} /></div>
              <div className='col-md-3 d-flex align-items-end'>
                <div className='form-check mb-2'>
                  <input type='checkbox' className='form-check-input' id='isLocked' checked={form.isLocked} onChange={(e) => setForm({ ...form, isLocked: e.target.checked })} />
                  <label className='form-check-label' htmlFor='isLocked'>Locked</label>
                </div>
              </div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </form>
        </Modal.Body>
        <Modal.Footer>
          <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
          <button type='submit' form='customer-form' className='btn btn-primary' disabled={isSaving}>
            {editingId === null ? "Add" : "Save"}
          </button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Customers'
            columns={[
              { key: "code", label: "Code", sortable: true },
              { key: "name", label: "Name", sortable: true },
              { key: "glAccount", label: "GL Account" },
              { key: "phone", label: "Phone" },
              { key: "email", label: "Email" },
              { key: "locked", label: "Locked" },
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
                <td>{item.isLocked && <span className='badge bg-danger bg-opacity-25 text-danger'>Locked</span>}</td>
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

export default CustomerSettingsLayer;
