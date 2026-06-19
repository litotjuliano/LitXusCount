import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { currenciesApi, type CurrencyUpsert } from "../api/settings/currencies";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "currencies"];

export function useCurrencySettings() {
  const queryClient = useQueryClient();

  const pagedQuery = usePaginatedQuery(queryKey, currenciesApi.list);
  const allActiveQuery = useQuery({ queryKey: [...queryKey, "all-active"], queryFn: currenciesApi.listAllActive });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey });
  };

  const createMutation = useMutation({
    mutationFn: (payload: CurrencyUpsert) => currenciesApi.create(payload),
    onSuccess: invalidate,
  });

  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: CurrencyUpsert }) => currenciesApi.edit(id, payload),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => currenciesApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, allActiveQuery, createMutation, editMutation, deleteMutation };
}
