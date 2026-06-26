import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { accountsApi, type AccAccountUpsert } from "../api/accounts/accounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["accounts"];

export function useAccounts() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, accountsApi.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: accountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: AccAccountUpsert) => accountsApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: AccAccountUpsert }) => accountsApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => accountsApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
