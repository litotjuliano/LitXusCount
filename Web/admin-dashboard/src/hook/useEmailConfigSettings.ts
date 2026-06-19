import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { emailConfigsApi, type EmailConfigUpsert } from "../api/settings/emailConfigs";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "email-configs"];

export function useEmailConfigSettings() {
  const queryClient = useQueryClient();

  const pagedQuery = usePaginatedQuery(queryKey, emailConfigsApi.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: emailConfigsApi.listAllActive });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
  };

  const createMutation = useMutation({
    mutationFn: (payload: EmailConfigUpsert) => emailConfigsApi.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: EmailConfigUpsert }) => emailConfigsApi.edit(id, payload),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => emailConfigsApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
