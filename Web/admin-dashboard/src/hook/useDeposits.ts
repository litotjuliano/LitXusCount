import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { depositsApi, type AccDepositCreate } from "../api/accounts/deposits";
import { accountsApi } from "../api/accounts/accounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["accounts", "deposits"];

export function useDeposits() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, depositsApi.list);
  const accountsQuery = useQuery({ queryKey: ["accounts", "all-active"], queryFn: accountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: AccDepositCreate) => depositsApi.create(payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => depositsApi.remove(id),
    onSuccess: () => {
      invalidate();
      queryClient.invalidateQueries({ queryKey: ["accounts"] });
    },
  });

  return { pagedQuery, accountsQuery, createMutation, deleteMutation };
}
