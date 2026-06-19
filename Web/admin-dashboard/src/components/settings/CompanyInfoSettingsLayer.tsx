import { useEffect, useState, type FormEvent } from "react";
import { useCompanyInfo } from "../../hook/useCompanyInfo";
import { useCurrencySettings } from "../../hook/useCurrencySettings";
import { useVatPercentageSettings } from "../../hook/useVatPercentageSettings";
import { useEmailConfigSettings } from "../../hook/useEmailConfigSettings";
import { usePermissions } from "../../hook/usePermissions";
import { Permissions } from "../../api/permissions";
import type { CompanyInfoUpdate } from "../../api/settings/companyInfo";
import { extractErrorMessage } from "./extractErrorMessage";

const emptyForm: CompanyInfoUpdate = {
  name: "",
  logoUrl: null,
  address: null,
  city: null,
  country: null,
  postCode: null,
  phone: null,
  mobile: null,
  email: null,
  fax: null,
  website: null,
  companyRegistrationNumber: null,
  vatRegistrationNumber: null,
  invoiceNumberPrefix: null,
  quoteNumberPrefix: null,
  termsAndConditions: null,
  isVatEnabled: false,
  vatTitle: null,
  isItemDiscountPercentage: false,
  currencyId: null,
  vatPercentageId: null,
  emailConfigId: null,
};

const toText = (value: string | null) => value ?? "";
const toNullable = (value: string) => (value === "" ? null : value);
const toNullableId = (value: string) => (value === "" ? null : Number(value));

const CompanyInfoSettingsLayer = () => {
  const { query, editMutation } = useCompanyInfo();
  const { hasPermission } = usePermissions();
  const canEdit = hasPermission(Permissions.Settings.CompanyInfo.Edit);
  const { allActiveQuery: currencies } = useCurrencySettings();
  const { allActiveQuery: vatPercentages } = useVatPercentageSettings();
  const { allActiveQuery: emailConfigs } = useEmailConfigSettings();

  const [form, setForm] = useState<CompanyInfoUpdate>(emptyForm);
  const [error, setError] = useState<string | null>(null);
  const [savedMessage, setSavedMessage] = useState<string | null>(null);

  useEffect(() => {
    if (query.data) {
      const { id: _id, ...rest } = query.data;
      setForm(rest);
    }
  }, [query.data]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSavedMessage(null);
    editMutation.mutate(form, {
      onSuccess: () => setSavedMessage("Company info saved."),
      onError: (err) => setError(extractErrorMessage(err)),
    });
  };

  if (query.isLoading) {
    return <div className='card'><div className='card-body'>Loading...</div></div>;
  }

  return (
    <form onSubmit={handleSubmit}>
      <div className='row gy-4'>
        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>Company details</h6>
            </div>
            <div className='card-body'>
              <div className='row gy-3'>
                <div className='col-md-6'>
                  <label className='form-label'>Company name</label>
                  <input
                    type='text'
                    className='form-control'
                    value={form.name}
                    onChange={(e) => setForm({ ...form, name: e.target.value })}
                    required
                  />
                </div>
                <div className='col-md-6'>
                  <label className='form-label'>Website</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.website)}
                    onChange={(e) => setForm({ ...form, website: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Phone</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.phone)}
                    onChange={(e) => setForm({ ...form, phone: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Mobile</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.mobile)}
                    onChange={(e) => setForm({ ...form, mobile: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Fax</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.fax)}
                    onChange={(e) => setForm({ ...form, fax: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-6'>
                  <label className='form-label'>Email</label>
                  <input
                    type='email'
                    className='form-control'
                    value={toText(form.email)}
                    onChange={(e) => setForm({ ...form, email: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-6'>
                  <label className='form-label'>Logo URL</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.logoUrl)}
                    onChange={(e) => setForm({ ...form, logoUrl: toNullable(e.target.value) })}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>Address</h6>
            </div>
            <div className='card-body'>
              <div className='row gy-3'>
                <div className='col-md-6'>
                  <label className='form-label'>Address</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.address)}
                    onChange={(e) => setForm({ ...form, address: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>City</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.city)}
                    onChange={(e) => setForm({ ...form, city: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>Post code</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.postCode)}
                    onChange={(e) => setForm({ ...form, postCode: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-6'>
                  <label className='form-label'>Country</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.country)}
                    onChange={(e) => setForm({ ...form, country: toNullable(e.target.value) })}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>Invoicing &amp; VAT</h6>
            </div>
            <div className='card-body'>
              <div className='row gy-3'>
                <div className='col-md-3'>
                  <label className='form-label'>Company reg. number</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.companyRegistrationNumber)}
                    onChange={(e) => setForm({ ...form, companyRegistrationNumber: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>VAT reg. number</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.vatRegistrationNumber)}
                    onChange={(e) => setForm({ ...form, vatRegistrationNumber: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>Invoice number prefix</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.invoiceNumberPrefix)}
                    onChange={(e) => setForm({ ...form, invoiceNumberPrefix: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-3'>
                  <label className='form-label'>Quote number prefix</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.quoteNumberPrefix)}
                    onChange={(e) => setForm({ ...form, quoteNumberPrefix: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>VAT title</label>
                  <input
                    type='text'
                    className='form-control'
                    value={toText(form.vatTitle)}
                    onChange={(e) => setForm({ ...form, vatTitle: toNullable(e.target.value) })}
                  />
                </div>
                <div className='col-md-4 d-flex align-items-center'>
                  <div className='form-check mt-24'>
                    <input
                      type='checkbox'
                      className='form-check-input'
                      id='isVatEnabled'
                      checked={form.isVatEnabled}
                      onChange={(e) => setForm({ ...form, isVatEnabled: e.target.checked })}
                    />
                    <label className='form-check-label' htmlFor='isVatEnabled'>
                      VAT enabled
                    </label>
                  </div>
                </div>
                <div className='col-md-4 d-flex align-items-center'>
                  <div className='form-check mt-24'>
                    <input
                      type='checkbox'
                      className='form-check-input'
                      id='isItemDiscountPercentage'
                      checked={form.isItemDiscountPercentage}
                      onChange={(e) => setForm({ ...form, isItemDiscountPercentage: e.target.checked })}
                    />
                    <label className='form-check-label' htmlFor='isItemDiscountPercentage'>
                      Item discount is a percentage
                    </label>
                  </div>
                </div>
                <div className='col-12'>
                  <label className='form-label'>Terms and conditions</label>
                  <textarea
                    className='form-control'
                    rows={3}
                    value={toText(form.termsAndConditions)}
                    onChange={(e) => setForm({ ...form, termsAndConditions: toNullable(e.target.value) })}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className='col-12'>
          <div className='card'>
            <div className='card-header'>
              <h6 className='card-title mb-0'>System defaults</h6>
            </div>
            <div className='card-body'>
              <div className='row gy-3'>
                <div className='col-md-4'>
                  <label className='form-label'>Default currency</label>
                  <select
                    className='form-select'
                    value={form.currencyId ?? ""}
                    onChange={(e) => setForm({ ...form, currencyId: toNullableId(e.target.value) })}
                  >
                    <option value=''>None</option>
                    {currencies.data?.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.name} ({c.code})
                      </option>
                    ))}
                  </select>
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Default VAT percentage</label>
                  <select
                    className='form-select'
                    value={form.vatPercentageId ?? ""}
                    onChange={(e) => setForm({ ...form, vatPercentageId: toNullableId(e.target.value) })}
                  >
                    <option value=''>None</option>
                    {vatPercentages.data?.map((v) => (
                      <option key={v.id} value={v.id}>
                        {v.name} ({v.percentage}%)
                      </option>
                    ))}
                  </select>
                </div>
                <div className='col-md-4'>
                  <label className='form-label'>Default email config</label>
                  <select
                    className='form-select'
                    value={form.emailConfigId ?? ""}
                    onChange={(e) => setForm({ ...form, emailConfigId: toNullableId(e.target.value) })}
                  >
                    <option value=''>None</option>
                    {emailConfigs.data?.map((e) => (
                      <option key={e.id} value={e.id}>
                        {e.email}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
          </div>
        </div>

        {canEdit && (
          <div className='col-12'>
            <button type='submit' className='btn btn-primary' disabled={editMutation.isPending}>
              {editMutation.isPending ? "Saving..." : "Save changes"}
            </button>
            {savedMessage && <span className='text-success-main ms-12'>{savedMessage}</span>}
            {error && <div className='text-danger-main text-sm mt-2'>{error}</div>}
          </div>
        )}
      </div>
    </form>
  );
};

export default CompanyInfoSettingsLayer;
