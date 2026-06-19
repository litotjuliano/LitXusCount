import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { companyInfoApi, type CompanyInfoUpdate } from "../api/settings/companyInfo";

const queryKey = ["settings", "company-info"];

export function useCompanyInfo() {
  const queryClient = useQueryClient();

  const query = useQuery({ queryKey, queryFn: companyInfoApi.get });

  const editMutation = useMutation({
    mutationFn: (payload: CompanyInfoUpdate) => companyInfoApi.edit(payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey }),
  });

  return { query, editMutation };
}
