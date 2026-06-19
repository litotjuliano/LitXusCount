import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { vatPercentagesApi, type VatPercentageUpsert } from "../api/settings/vatPercentages";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "vat-percentages"];

export function useVatPercentageSettings() {
  const queryClient = useQueryClient();

  const pagedQuery = usePaginatedQuery(queryKey, vatPercentagesApi.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: vatPercentagesApi.listAllActive });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
  };

  const createMutation = useMutation({
    mutationFn: (payload: VatPercentageUpsert) => vatPercentagesApi.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: VatPercentageUpsert }) =>
      vatPercentagesApi.edit(id, payload),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => vatPercentagesApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
