import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { productsApi, type ProductUpsert } from "../api/settings/products";
import { glAccountsApi } from "../api/settings/glAccounts";
import { suppliersApi } from "../api/settings/suppliers";
import { createLookupApi } from "../api/settings/lookupApi";
import { usePaginatedQuery } from "./usePaginatedQuery";

const queryKey = ["settings", "products"];
const categoriesApi = createLookupApi("categories");
const uomApi = createLookupApi("units-of-measure");

export function useProductSettings() {
  const queryClient = useQueryClient();
  const pagedQuery = usePaginatedQuery(queryKey, productsApi.list);
  const glAccountsQuery = useQuery({ queryKey: ["settings", "gl-accounts", "all-active"], queryFn: glAccountsApi.listAllActive });
  const suppliersQuery = useQuery({ queryKey: ["settings", "suppliers", "all-active"], queryFn: suppliersApi.listAllActive });
  const categoriesQuery = useQuery({ queryKey: ["settings", "categories", "all-active"], queryFn: categoriesApi.listAllActive });
  const uomQuery = useQuery({ queryKey: ["settings", "units-of-measure", "all-active"], queryFn: uomApi.listAllActive });

  const invalidate = () => queryClient.invalidateQueries({ queryKey });

  const createMutation = useMutation({
    mutationFn: (payload: ProductUpsert) => productsApi.create(payload),
    onSuccess: invalidate,
  });
  const editMutation = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: ProductUpsert }) => productsApi.edit(id, payload),
    onSuccess: invalidate,
  });
  const deleteMutation = useMutation({
    mutationFn: (id: number) => productsApi.remove(id),
    onSuccess: invalidate,
  });

  return { pagedQuery, glAccountsQuery, suppliersQuery, categoriesQuery, uomQuery, createMutation, editMutation, deleteMutation };
}
