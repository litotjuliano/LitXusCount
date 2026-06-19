export interface LookupItem {
  id: number;
  name: string;
  description: string | null;
  isActive: boolean;
}

export interface LookupUpsert {
  name: string;
  description: string | null;
}
