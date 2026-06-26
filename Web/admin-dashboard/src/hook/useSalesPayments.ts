import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { salesPaymentsApi, type SalesPaymentRecordCreateDto } from "../api/sales/salesPayments";

export function useSalesPayments(invoiceId: number) {
  const queryClient = useQueryClient();
  const queryKey = ["sales", "payments", invoiceId];

  const paymentsQuery = useQuery({
    queryKey,
    queryFn: () => salesPaymentsApi.getByInvoice(invoiceId),
    enabled: invoiceId > 0,
  });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
    queryClient.invalidateQueries({ queryKey: ["sales", "invoices", invoiceId] });
    queryClient.invalidateQueries({ queryKey: ["sales", "invoices"] });
    queryClient.invalidateQueries({ queryKey: ["accounts"] });
  };

  const recordMutation = useMutation({
    mutationFn: (dto: SalesPaymentRecordCreateDto) => salesPaymentsApi.recordPayment(dto),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => salesPaymentsApi.deletePayment(id),
    onSuccess: invalidate,
  });

  return { paymentsQuery, recordMutation, deleteMutation };
}
