import { useState, type FormEvent } from "react";
import { useQuery } from "@tanstack/react-query";
import { Icon } from "@iconify/react/dist/iconify.js";
import { Modal } from "react-bootstrap";
import { useProductSettings } from "../../hook/useProductSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { ProductItem } from "../../api/settings/products";
import { lhdnClassificationCodesApi } from "../../api/settings/lhdnClassificationCodes";
import { lhdnTaxTypesApi } from "../../api/settings/lhdnTaxTypes";
import { extractErrorMessage } from "./extractErrorMessage";
import PaginatedTable from "./PaginatedTable";

interface FormState {
  code: string; code2: string; parentProductCode: string; categoryId: number | "";
  description: string;
  salesCogsAccountId: number | ""; salesRevenueAccountId: number | "";
  purchaseCostAccountId: number | ""; purchaseAccountId: number | "";
  salesTaxCode: string; purchaseTaxCode: string;
  defaultLhdnClassificationCode: string; defaultLhdnTaxTypeCode: string;
  defaultSupplierId: number | ""; mainUnitOfMeasureId: number | ""; altUnitOfMeasureId: number | "";
  conversionFactor: number; shelfLifeDays: number;
  unitCostPrice: number; unitSellingPrice: number;
  unitSellingPrice2: number; minSalesQty2: number;
  unitSellingPrice3: number; minSalesQty3: number;
  promoCode: string; promoFromDate: string; promoToDate: string;
  minQty: number; maxQty: number; reorderQty: number; leadTimeDays: number;
  packagingQty: string; remark: string; imageRef: string;
}

const emptyForm: FormState = {
  code: "", code2: "", parentProductCode: "", categoryId: "", description: "",
  salesCogsAccountId: "", salesRevenueAccountId: "", purchaseCostAccountId: "", purchaseAccountId: "",
  salesTaxCode: "", purchaseTaxCode: "", defaultLhdnClassificationCode: "", defaultLhdnTaxTypeCode: "",
  defaultSupplierId: "", mainUnitOfMeasureId: "", altUnitOfMeasureId: "",
  conversionFactor: 1, shelfLifeDays: 0,
  unitCostPrice: 0, unitSellingPrice: 0, unitSellingPrice2: 0, minSalesQty2: 0, unitSellingPrice3: 0, minSalesQty3: 0,
  promoCode: "", promoFromDate: "", promoToDate: "",
  minQty: 0, maxQty: 0, reorderQty: 0, leadTimeDays: 0,
  packagingQty: "", remark: "", imageRef: "",
};

const ProductSettingsLayer = () => {
  const { pagedQuery, glAccountsQuery, suppliersQuery, categoriesQuery, uomQuery, createMutation, editMutation, deleteMutation } = useProductSettings();
  const classificationCodesQuery = useQuery({ queryKey: ["settings", "lhdn-classification-codes", "all-active"], queryFn: lhdnClassificationCodesApi.listAllActive });
  const taxTypesQuery = useQuery({ queryKey: ["settings", "lhdn-tax-types", "all-active"], queryFn: lhdnTaxTypesApi.listAllActive });
  const { hasPermission } = usePermissions();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);

  const canCreate = hasPermission(Permissions.Settings.Product.Create);
  const canEdit = hasPermission(Permissions.Settings.Product.Edit);
  const canDelete = hasPermission(Permissions.Settings.Product.Delete);
  const isSaving = createMutation.isPending || editMutation.isPending;
  const glAccounts = glAccountsQuery.data ?? [];
  const suppliers = suppliersQuery.data ?? [];
  const categories = categoriesQuery.data ?? [];
  const uoms = uomQuery.data ?? [];
  const classificationCodes = classificationCodesQuery.data ?? [];
  const taxTypes = taxTypesQuery.data ?? [];

  const n = (v: string) => v || null;
  const ni = (v: number | "") => v === "" ? null : Number(v);
  const f = (k: keyof FormState, v: string | number) => setForm((prev) => ({ ...prev, [k]: v }));
  const sel = (v: string) => v === "" ? "" : Number(v) as number | "";

  const openAdd = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(true); };

  const startEdit = (item: ProductItem) => {
    setEditingId(item.id);
    setForm({
      code: item.code, code2: item.code2 ?? "", parentProductCode: item.parentProductCode ?? "",
      categoryId: item.categoryId ?? "", description: item.description ?? "",
      salesCogsAccountId: item.salesCogsAccountId ?? "", salesRevenueAccountId: item.salesRevenueAccountId ?? "",
      purchaseCostAccountId: item.purchaseCostAccountId ?? "", purchaseAccountId: item.purchaseAccountId ?? "",
      salesTaxCode: item.salesTaxCode ?? "", purchaseTaxCode: item.purchaseTaxCode ?? "",
      defaultLhdnClassificationCode: item.defaultLhdnClassificationCode ?? "",
      defaultLhdnTaxTypeCode: item.defaultLhdnTaxTypeCode ?? "",
      defaultSupplierId: item.defaultSupplierId ?? "", mainUnitOfMeasureId: item.mainUnitOfMeasureId ?? "", altUnitOfMeasureId: item.altUnitOfMeasureId ?? "",
      conversionFactor: item.conversionFactor, shelfLifeDays: item.shelfLifeDays,
      unitCostPrice: item.unitCostPrice, unitSellingPrice: item.unitSellingPrice,
      unitSellingPrice2: item.unitSellingPrice2, minSalesQty2: item.minSalesQty2,
      unitSellingPrice3: item.unitSellingPrice3, minSalesQty3: item.minSalesQty3,
      promoCode: item.promoCode ?? "", promoFromDate: item.promoFromDate?.slice(0, 10) ?? "", promoToDate: item.promoToDate?.slice(0, 10) ?? "",
      minQty: item.minQty, maxQty: item.maxQty, reorderQty: item.reorderQty, leadTimeDays: item.leadTimeDays,
      packagingQty: item.packagingQty ?? "", remark: item.remark ?? "", imageRef: item.imageRef ?? "",
    });
    setError(null);
    setShowModal(true);
  };

  const closeModal = () => { setForm(emptyForm); setEditingId(null); setError(null); setShowModal(false); };

  const buildPayload = () => ({
    code: form.code, code2: n(form.code2), parentProductCode: n(form.parentProductCode),
    categoryId: ni(form.categoryId), description: n(form.description),
    salesCogsAccountId: ni(form.salesCogsAccountId), salesRevenueAccountId: ni(form.salesRevenueAccountId),
    purchaseCostAccountId: ni(form.purchaseCostAccountId), purchaseAccountId: ni(form.purchaseAccountId),
    salesTaxCode: n(form.salesTaxCode), purchaseTaxCode: n(form.purchaseTaxCode),
    defaultLhdnClassificationCode: n(form.defaultLhdnClassificationCode),
    defaultLhdnTaxTypeCode: n(form.defaultLhdnTaxTypeCode),
    defaultSupplierId: ni(form.defaultSupplierId), mainUnitOfMeasureId: ni(form.mainUnitOfMeasureId), altUnitOfMeasureId: ni(form.altUnitOfMeasureId),
    conversionFactor: form.conversionFactor, shelfLifeDays: form.shelfLifeDays,
    unitCostPrice: form.unitCostPrice, unitSellingPrice: form.unitSellingPrice,
    unitSellingPrice2: form.unitSellingPrice2, minSalesQty2: form.minSalesQty2,
    unitSellingPrice3: form.unitSellingPrice3, minSalesQty3: form.minSalesQty3,
    promoCode: n(form.promoCode), promoFromDate: n(form.promoFromDate), promoToDate: n(form.promoToDate),
    minQty: form.minQty, maxQty: form.maxQty, reorderQty: form.reorderQty, leadTimeDays: form.leadTimeDays,
    packagingQty: n(form.packagingQty), remark: n(form.remark), imageRef: n(form.imageRef),
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault(); setError(null);
    if (editingId === null) {
      createMutation.mutate(buildPayload(), { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    } else {
      editMutation.mutate({ id: editingId, payload: buildPayload() }, { onSuccess: closeModal, onError: (err) => setError(extractErrorMessage(err)) });
    }
  };

  const glAccountSelect = (label: string, key: keyof FormState) => (
    <div className='col-md-6'>
      <label className='form-label'>{label}</label>
      <select className='form-select' value={form[key] as number | ""} onChange={(e) => setForm({ ...form, [key]: sel(e.target.value) })}>
        <option value=''>— None —</option>
        {glAccounts.map(a => <option key={a.id} value={a.id}>{a.code} — {a.name}</option>)}
      </select>
    </div>
  );

  return (
    <>
      <Modal show={showModal} onHide={closeModal} centered size='xl' scrollable>
        <Modal.Header closeButton>
          <Modal.Title className='h6'>{editingId === null ? "Add Product" : "Edit Product"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form id='product-form' onSubmit={handleSubmit}>
            <div className='row gy-3'>
              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>General</p></div>
              <div className='col-md-3'><label className='form-label'>Code <span className='text-danger'>*</span></label><input type='text' className='form-control' value={form.code} onChange={(e) => f("code", e.target.value)} required autoFocus /></div>
              <div className='col-md-3'><label className='form-label'>Code 2</label><input type='text' className='form-control' value={form.code2} onChange={(e) => f("code2", e.target.value)} /></div>
              <div className='col-md-3'><label className='form-label'>Parent Code</label><input type='text' className='form-control' value={form.parentProductCode} onChange={(e) => f("parentProductCode", e.target.value)} /></div>
              <div className='col-md-3'>
                <label className='form-label'>Category</label>
                <select className='form-select' value={form.categoryId} onChange={(e) => setForm({ ...form, categoryId: sel(e.target.value) })}>
                  <option value=''>— None —</option>
                  {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
              <div className='col-12'><label className='form-label'>Description</label><input type='text' className='form-control' value={form.description} onChange={(e) => f("description", e.target.value)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>GL Accounts</p></div>
              {glAccountSelect("Sales COGS", "salesCogsAccountId")}
              {glAccountSelect("Sales Revenue", "salesRevenueAccountId")}
              {glAccountSelect("Purchase Cost", "purchaseCostAccountId")}
              {glAccountSelect("Purchase Account", "purchaseAccountId")}

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Logistics</p></div>
              <div className='col-md-3'><label className='form-label'>Sales Tax Code</label><input type='text' className='form-control' value={form.salesTaxCode} onChange={(e) => f("salesTaxCode", e.target.value)} /></div>
              <div className='col-md-3'><label className='form-label'>Purchase Tax Code</label><input type='text' className='form-control' value={form.purchaseTaxCode} onChange={(e) => f("purchaseTaxCode", e.target.value)} /></div>
              <div className='col-md-6'>
                <label className='form-label'>Default Supplier</label>
                <select className='form-select' value={form.defaultSupplierId} onChange={(e) => setForm({ ...form, defaultSupplierId: sel(e.target.value) })}>
                  <option value=''>— None —</option>
                  {suppliers.map(s => <option key={s.id} value={s.id}>{s.code} — {s.name}</option>)}
                </select>
              </div>
              <div className='col-md-4'>
                <label className='form-label'>Main UOM</label>
                <select className='form-select' value={form.mainUnitOfMeasureId} onChange={(e) => setForm({ ...form, mainUnitOfMeasureId: sel(e.target.value) })}>
                  <option value=''>— None —</option>
                  {uoms.map(u => <option key={u.id} value={u.id}>{u.name}</option>)}
                </select>
              </div>
              <div className='col-md-4'>
                <label className='form-label'>Alt UOM</label>
                <select className='form-select' value={form.altUnitOfMeasureId} onChange={(e) => setForm({ ...form, altUnitOfMeasureId: sel(e.target.value) })}>
                  <option value=''>— None —</option>
                  {uoms.map(u => <option key={u.id} value={u.id}>{u.name}</option>)}
                </select>
              </div>
              <div className='col-md-2'><label className='form-label'>Conv. Factor</label><input type='number' className='form-control' step='0.0001' value={form.conversionFactor} onChange={(e) => f("conversionFactor", parseFloat(e.target.value) || 1)} /></div>
              <div className='col-md-2'><label className='form-label'>Shelf Life (days)</label><input type='number' className='form-control' value={form.shelfLifeDays} onChange={(e) => f("shelfLifeDays", parseInt(e.target.value) || 0)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>LHDN Defaults (MyInvois)</p></div>
              <div className='col-md-6'>
                <label className='form-label'>Classification Code</label>
                <select className='form-select' value={form.defaultLhdnClassificationCode} onChange={(e) => f("defaultLhdnClassificationCode", e.target.value)}>
                  <option value=''>— None —</option>
                  {classificationCodes.map(c => <option key={c.id} value={c.code}>{c.code} — {c.description}</option>)}
                </select>
              </div>
              <div className='col-md-6'>
                <label className='form-label'>Tax Type</label>
                <select className='form-select' value={form.defaultLhdnTaxTypeCode} onChange={(e) => f("defaultLhdnTaxTypeCode", e.target.value)}>
                  <option value=''>— None —</option>
                  {taxTypes.map(t => <option key={t.id} value={t.code}>{t.code} — {t.description}</option>)}
                </select>
              </div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Pricing</p></div>
              <div className='col-md-4'><label className='form-label'>Cost Price</label><input type='number' className='form-control' step='0.0001' value={form.unitCostPrice} onChange={(e) => f("unitCostPrice", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Selling Price 1</label><input type='number' className='form-control' step='0.0001' value={form.unitSellingPrice} onChange={(e) => f("unitSellingPrice", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Selling Price 2</label><input type='number' className='form-control' step='0.0001' value={form.unitSellingPrice2} onChange={(e) => f("unitSellingPrice2", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Min Qty for Price 2</label><input type='number' className='form-control' step='0.01' value={form.minSalesQty2} onChange={(e) => f("minSalesQty2", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Selling Price 3</label><input type='number' className='form-control' step='0.0001' value={form.unitSellingPrice3} onChange={(e) => f("unitSellingPrice3", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Min Qty for Price 3</label><input type='number' className='form-control' step='0.01' value={form.minSalesQty3} onChange={(e) => f("minSalesQty3", parseFloat(e.target.value) || 0)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Promotion</p></div>
              <div className='col-md-4'><label className='form-label'>Promo Code</label><input type='text' className='form-control' value={form.promoCode} onChange={(e) => f("promoCode", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Promo From</label><input type='date' className='form-control' value={form.promoFromDate} onChange={(e) => f("promoFromDate", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Promo To</label><input type='date' className='form-control' value={form.promoToDate} onChange={(e) => f("promoToDate", e.target.value)} /></div>

              <div className='col-12'><p className='small text-muted fw-semibold mb-0 border-bottom pb-1'>Stock Control</p></div>
              <div className='col-md-3'><label className='form-label'>Min Qty</label><input type='number' className='form-control' step='0.01' value={form.minQty} onChange={(e) => f("minQty", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-3'><label className='form-label'>Max Qty</label><input type='number' className='form-control' step='0.01' value={form.maxQty} onChange={(e) => f("maxQty", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-3'><label className='form-label'>Reorder Qty</label><input type='number' className='form-control' step='0.01' value={form.reorderQty} onChange={(e) => f("reorderQty", parseFloat(e.target.value) || 0)} /></div>
              <div className='col-md-3'><label className='form-label'>Lead Time (days)</label><input type='number' className='form-control' value={form.leadTimeDays} onChange={(e) => f("leadTimeDays", parseInt(e.target.value) || 0)} /></div>
              <div className='col-md-4'><label className='form-label'>Packaging Qty</label><input type='text' className='form-control' value={form.packagingQty} onChange={(e) => f("packagingQty", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Remark</label><input type='text' className='form-control' value={form.remark} onChange={(e) => f("remark", e.target.value)} /></div>
              <div className='col-md-4'><label className='form-label'>Image Ref</label><input type='text' className='form-control' value={form.imageRef} onChange={(e) => f("imageRef", e.target.value)} /></div>
            </div>
            {error && <div className='text-danger small mt-3'>{error}</div>}
          </form>
        </Modal.Body>
        <Modal.Footer>
          <button type='button' className='btn btn-outline-secondary' onClick={closeModal}>Cancel</button>
          <button type='submit' form='product-form' className='btn btn-primary' disabled={isSaving}>
            {editingId === null ? "Add" : "Save"}
          </button>
        </Modal.Footer>
      </Modal>

      <div className='row'>
        <div className='col-12'>
          <PaginatedTable
            title='Products'
            columns={[
              { key: "code", label: "Code", sortable: true },
              { key: "description", label: "Description", sortable: true },
              { key: "category", label: "Category" },
              { key: "costPrice", label: "Cost Price" },
              { key: "sellingPrice", label: "Selling Price" },
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
                <td>{item.description ?? "—"}</td>
                <td>{item.categoryName ?? "—"}</td>
                <td>{item.unitCostPrice.toFixed(4)}</td>
                <td>{item.unitSellingPrice.toFixed(4)}</td>
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

export default ProductSettingsLayer;
