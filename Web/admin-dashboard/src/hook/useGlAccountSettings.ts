import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { glAccountsApi, type GlAccountUpsert } from "../api/settings/glAccounts";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "gl-accounts"];

export function useGlAccountSettings() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, glAccountsApi.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: glAccountsApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: GlAccountUpsert) => glAccountsApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: GlAccountUpsert }) => glAccountsApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => glAccountsApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
