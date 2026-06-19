import { useMemo } from "react";
import { decodeJwtPayload } from "../api/jwt";
import { authStorage } from "../api/authStorage";

const PERMISSION_CLAIM = "permission";

export function usePermissions() {
  const permissions = useMemo(() => {
    const token = authStorage.getAccessToken();
    if (!token) {
      return new Set<string>();
    }

    const payload = decodeJwtPayload(token);
    const claim = payload?.[PERMISSION_CLAIM];

    if (Array.isArray(claim)) {
      return new Set(claim.map(String));
    }
    if (typeof claim === "string") {
      return new Set([claim]);
    }
    return new Set<string>();
  }, []);

  const hasPermission = (name: string) => permissions.has(name);

  return { permissions, hasPermission };
}
