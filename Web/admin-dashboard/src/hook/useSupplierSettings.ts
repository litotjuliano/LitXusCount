import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { suppliersApi, type SupplierUpsert } from "../api/settings/suppliers";
import { glAccountsApi } from "../api/settings/glAccounts";
import { currenciesApi } from "../api/settings/currencies";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "suppliers"];

export function useSupplierSettings() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, suppliersApi.list);
  const glAccountsQuery = useQuery({ queryKey: ["settings", "gl-accounts", "all-active"], queryFn: glAccountsApi.listAllActive });
  const currenciesQuery = useQuery({ queryKey: ["settings", "currencies", "all-active"], queryFn: currenciesApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: SupplierUpsert) => suppliersApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: SupplierUpsert }) => suppliersApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => suppliersApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, glAccountsQuery, currenciesQuery, createMutation, editMutation, deleteMutation };
}
