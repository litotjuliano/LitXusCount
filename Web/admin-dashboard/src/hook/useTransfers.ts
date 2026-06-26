import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { transfersApi, type AccTransferCreate } from "../api/accounts/transfers";
import { accountsApi } from "../api/accounts/accounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["accounts", "transfers"];

export function useTransfers() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, transfersApi.list);
  const accountsQuery = useQuery({ queryKey: ["accounts", "all-active"], queryFn: accountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: AccTransferCreate) => transfersApi.create(payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => transfersApi.remove(id),
    onSuccess: () => {
      invalidate();
      queryClient.invalidateQueries({ queryKey: ["accounts"] });
    },
  });

  return { pagedQuery, accountsQuery, createMutation, deleteMutation };
}
