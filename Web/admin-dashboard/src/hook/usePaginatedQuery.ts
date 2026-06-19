import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { PagedQuery, PagedResult } from "../api/settings/paging";
import { useDebouncedValue } from "./useDebouncedValue";

export function usePaginatedQuery<T>(queryKeyBase: unknown[], fetcher: (query: PagedQuery) => Promise<PagedResult<T>>) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSizeState] = useState(20);
  const [search, setSearchState] = useState("");
  const [sortBy, setSortBy] = useState<string | undefined>(undefined);
  const [sortDescending, setSortDescending] = useState(false);

  const debouncedSearch = useDebouncedValue(search, 350);

  const query: PagedQuery = { page, pageSize, search: debouncedSearch || undefined, sortBy, sortDescending };

  const result = useQuery({
    queryKey: [...queryKeyBase, "paged", query],
    queryFn: () => fetcher(query),
  });

  const setSearch = (value: string) => {
    setSearchState(value);
    setPage(1);
  };

  const setPageSize = (size: number) => {
    setPageSizeState(size);
    setPage(1);
  };

  const toggleSort = (column: string) => {
    if (sortBy === column) {
      setSortDescending((prev) => !prev);
    } else {
      setSortBy(column);
      setSortDescending(false);
    }
    setPage(1);
  };

  return {
    result,
    page,
    setPage,
    pageSize,
    setPageSize,
    search,
    setSearch,
    sortBy,
    sortDescending,
    toggleSort,
  };
}
