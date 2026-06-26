import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { expensesApi, type AccExpenseCreate } from "../api/accounts/expenses";
import { accountsApi } from "../api/accounts/accounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["accounts", "expenses"];

export function useExpenses() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, expensesApi.list);
  const accountsQuery = useQuery({ queryKey: ["accounts", "all-active"], queryFn: accountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: AccExpenseCreate) => expensesApi.create(payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => expensesApi.remove(id),
    onSuccess: () => {
      invalidate();
      queryClient.invalidateQueries({ queryKey: ["accounts"] });
    },
  });

  return { pagedQuery, accountsQuery, createMutation, deleteMutation };
}
