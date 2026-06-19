import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createLookupApi } from "../api/settings/lookupApi";
import type { LookupUpsert } from "../api/settings/types";
import { usePaginatedQuery } from "./usePaginatedQuery";

export function useLookupSettings(resourcePath: string) {
  const api = createLookupApi(resourcePath);
  const queryClient = useQueryClient();
  const queryKey = ["settings", resourcePath];

  const pagedQuery = usePaginatedQuery(queryKey, api.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: api.listAllActive });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
  };

  const createMutation = useMutation({
    mutationFn: (payload: LookupUpsert) => api.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: LookupUpsert }) => api.edit(id, payload),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
