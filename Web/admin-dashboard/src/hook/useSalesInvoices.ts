import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  salesInvoicesApi,
  type SalesInvoiceCategory,
  type SalesInvoiceCreateDto,
  type SalesInvoiceHeaderUpdateDto,
} from "../api/sales/salesInvoices";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["sales", "invoices"];

export function useSalesInvoices(category?: SalesInvoiceCategory | null) {
  const queryClient = useQueryClient();

  const pagedQuery = usePaginatedQuery(queryKey, (params) =>
    salesInvoicesApi.getPaged({ ...params, category })
  );

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (dto: SalesInvoiceCreateDto) => salesInvoicesApi.createDraft(dto),
    onSuccess: invalidate,
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: SalesInvoiceHeaderUpdateDto }) =>
      salesInvoicesApi.updateHeader(id, dto),
    onSuccess: invalidate,
  });

  const promoteMutation = useMutation({
    mutationFn: ({ id, targetCategory }: { id: number; targetCategory: SalesInvoiceCategory }) =>
      salesInvoicesApi.promote(id, targetCategory),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => salesInvoicesApi.delete(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, createMutation, updateMutation, promoteMutation, deleteMutation };
}

export function useSalesInvoiceDetail(id: number) {
  return useQuery({
    queryKey: ["sales", "invoices", id],
    queryFn: () => salesInvoicesApi.getById(id),
    enabled: id > 0,
  });
}
