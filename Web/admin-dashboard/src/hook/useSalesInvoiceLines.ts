import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { salesInvoiceLinesApi, type SalesInvoiceLineCreateDto } from "../api/sales/salesInvoiceLines";

export function useSalesInvoiceLines(invoiceId: number) {
  const queryClient = useQueryClient();
  const queryKey = ["sales", "invoice-lines", invoiceId];

  const linesQuery = useQuery({
    queryKey,
    queryFn: () => salesInvoiceLinesApi.getByInvoice(invoiceId),
    enabled: invoiceId > 0,
  });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
    queryClient.invalidateQueries({ queryKey: ["sales", "invoices", invoiceId] });
    queryClient.invalidateQueries({ queryKey: ["sales", "invoices"] });
  };

  const addMutation = useMutation({
    mutationFn: (dto: SalesInvoiceLineCreateDto) => salesInvoiceLinesApi.addLine(dto),
    onSuccess: invalidate,
  });

  const removeMutation = useMutation({
    mutationFn: (id: number) => salesInvoiceLinesApi.removeLine(id),
    onSuccess: invalidate,
  });

  return { linesQuery, addMutation, removeMutation };
}
