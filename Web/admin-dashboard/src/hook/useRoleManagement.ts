import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { rolesApi, type RoleUpsertRequest } from "../api/admin/roles";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["admin", "roles"];

export function useRoleManagement() {
  const queryClient = useQueryClient();

  const pagedQuery = usePaginatedQuery(queryKey, rolesApi.list);
  const catalogQuery = useQuery({ queryKey: [...queryKey, "catalog"], queryFn: rolesApi.permissionsCatalog });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: RoleUpsertRequest) => rolesApi.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: RoleUpsertRequest }) => rolesApi.edit(id, payload),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => rolesApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, catalogQuery, createMutation, editMutation, deleteMutation };
}
