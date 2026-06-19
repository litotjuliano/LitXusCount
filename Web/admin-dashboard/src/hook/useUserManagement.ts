import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi, type UserCreateRequest, type UserEditRequest } from "../api/admin/users";
import { usePaginatedQuery } from "./usePaginatedQuery";

export function useUserManagement(includeInactive: boolean) {
  const queryClient = useQueryClient();
  const queryKeyBase = ["admin", "users", includeInactive];

  const pagedQuery = usePaginatedQuery(queryKeyBase, (query) => usersApi.list(query, includeInactive));

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["admin", "users"] });

  const createMutation = useMutation({
    mutationFn: (payload: UserCreateRequest) => usersApi.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UserEditRequest }) => usersApi.edit(id, payload),
    onSuccess: invalidate,
  });

  const deactivateMutation = useMutation({
    mutationFn: (id: string) => usersApi.deactivate(id),
    onSuccess: invalidate,
  });

  const reactivateMutation = useMutation({
    mutationFn: (id: string) => usersApi.reactivate(id),
    onSuccess: invalidate,
  });

  const resetPasswordMutation = useMutation({
    mutationFn: ({ id, newPassword }: { id: string; newPassword: string }) => usersApi.resetPassword(id, newPassword),
    onSuccess: invalidate,
  });

  return { pagedQuery, createMutation, editMutation, deactivateMutation, reactivateMutation, resetPasswordMutation };
}
