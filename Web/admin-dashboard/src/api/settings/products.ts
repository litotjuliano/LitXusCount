import { apiClient } from "../client";
import type { PagedQuery, PagedResult } from "./paging";

export interface ProductItem {
  id: number;
  code: string;
  code2: string | null;
  parentProductCode: string | null;
  categoryId: number | null;
  categoryName: string | null;
  description: string | null;
  salesCogsAccountId: number | null;
  salesCogsAccountName: string | null;
  salesRevenueAccountId: number | null;
  salesRevenueAccountName: string | null;
  purchaseCostAccountId: number | null;
  purchaseCostAccountName: string | null;
  purchaseAccountId: number | null;
  purchaseAccountName: string | null;
  salesTaxCode: string | null;
  purchaseTaxCode: string | null;
  defaultSupplierId: number | null;
  defaultSupplierName: string | null;
  mainUnitOfMeasureId: number | null;
  mainUnitOfMeasureName: string | null;
  altUnitOfMeasureId: number | null;
  altUnitOfMeasureName: string | null;
  conversionFactor: number;
  shelfLifeDays: number;
  unitCostPrice: number;
  unitSellingPrice: number;
  unitSellingPrice2: number;
  minSalesQty2: number;
  unitSellingPrice3: number;
  minSalesQty3: number;
  promoCode: string | null;
  promoFromDate: string | null;
  promoToDate: string | null;
  minQty: number;
  maxQty: number;
  reorderQty: number;
  leadTimeDays: number;
  packagingQty: string | null;
  remark: string | null;
  imageRef: string | null;
  isActive: boolean;
}

export interface ProductUpsert {
  code: string;
  code2: string | null;
  parentProductCode: string | null;
  categoryId: number | null;
  description: string | null;
  salesCogsAccountId: number | null;
  salesRevenueAccountId: number | null;
  purchaseCostAccountId: number | null;
  purchaseAccountId: number | null;
  salesTaxCode: string | null;
  purchaseTaxCode: string | null;
  defaultSupplierId: number | null;
  mainUnitOfMeasureId: number | null;
  altUnitOfMeasureId: number | null;
  conversionFactor: number;
  shelfLifeDays: number;
  unitCostPrice: number;
  unitSellingPrice: number;
  unitSellingPrice2: number;
  minSalesQty2: number;
  unitSellingPrice3: number;
  minSalesQty3: number;
  promoCode: string | null;
  promoFromDate: string | null;
  promoToDate: string | null;
  minQty: number;
  maxQty: number;
  reorderQty: number;
  leadTimeDays: number;
  packagingQty: string | null;
  remark: string | null;
  imageRef: string | null;
}

const base = "/api/settings/products";

export const productsApi = {
  list: async (query: PagedQuery): Promise<PagedResult<ProductItem>> =>
    (await apiClient.get<PagedResult<ProductItem>>(base, { params: query })).data,
  listAllActive: async (): Promise<ProductItem[]> =>
    (await apiClient.get<ProductItem[]>(`${base}/all-active`)).data,
  create: async (payload: ProductUpsert): Promise<ProductItem> =>
    (await apiClient.post<ProductItem>(base, payload)).data,
  edit: async (id: number, payload: ProductUpsert): Promise<ProductItem> =>
    (await apiClient.put<ProductItem>(`${base}/${id}`, payload)).data,
  remove: async (id: number): Promise<void> => { await apiClient.delete(`${base}/${id}`); },
};
