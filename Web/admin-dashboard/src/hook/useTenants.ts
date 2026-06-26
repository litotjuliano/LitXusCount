import { useMutation, useQueryClient } from "@tanstack/react-query";
import { tenantsApi, type TenantUpsert } from "../api/tenants";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["admin", "tenants"];

export function useTenants() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, tenantsApi.list);
  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: TenantUpsert) => tenantsApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: TenantUpsert }) =>
      tenantsApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const toggleActiveMutation = useMutation({
    mutationFn: (id: number) => tenantsApi.toggleActive(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, createMutation, editMutation, toggleActiveMutation };
}
