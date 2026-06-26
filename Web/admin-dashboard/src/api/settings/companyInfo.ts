import { apiClient } from "../client";

export interface CompanyInfo {
  id: number;
  name: string;
  logoUrl: string | null;
  address: string | null;
  city: string | null;
  country: string | null;
  postCode: string | null;
  phone: string | null;
  mobile: string | null;
  email: string | null;
  fax: string | null;
  website: string | null;
  companyRegistrationNumber: string | null;
  vatRegistrationNumber: string | null;
  invoiceNumberPrefix: string | null;
  quoteNumberPrefix: string | null;
  termsAndConditions: string | null;
  isVatEnabled: boolean;
  vatTitle: string | null;
  isItemDiscountPercentage: boolean;
  currencyId: number | null;
  vatPercentageId: number | null;
  emailConfigId: number | null;
}

export type CompanyInfoUpdate = Omit<CompanyInfo, "id">;

const base = "/api/settings/company-info";

export const companyInfoApi = {
  get: async (): Promise<CompanyInfo> => (await apiClient.get<CompanyInfo>(base)).data,
  edit: async (payload: CompanyInfoUpdate): Promise<CompanyInfo> =>
    (await apiClient.put<CompanyInfo>(base, payload)).data,
  uploadLogo: async (file: File): Promise<{ url: string }> => {
    const body = new FormData();
    body.append('file', file);
    return (await apiClient.post<{ url: string }>(`${base}/logo`, body)).data;
  },
};
