import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { customersApi, type CustomerUpsert } from "../api/settings/customers";
import { glAccountsApi } from "../api/settings/glAccounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "customers"];

export function useCustomerSettings() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, customersApi.list);
  const glAccountsQuery = useQuery({ queryKey: ["settings", "gl-accounts", "all-active"], queryFn: glAccountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: CustomerUpsert) => customersApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: CustomerUpsert }) => customersApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => customersApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, glAccountsQuery, createMutation, editMutation, deleteMutation };
}
