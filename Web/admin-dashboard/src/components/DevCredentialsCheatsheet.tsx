import { Icon } from "@iconify/react/dist/iconify.js";

export interface DevCredential {
  email: string;
  password: string;
  role: string;
  roleVariant: "danger" | "warning" | "primary";
}

export const DEV_CREDENTIALS: DevCredential[] = [
  {
    email: "admin@litxuscount.local",
    password: "DevPassw0rd!",
    role: "Super Admin (System)",
    roleVariant: "danger",
  },
  {
    email: "companyadmin@litxuscount.local",
    password: "DevPassw0rd!",
    role: "Admin (Company Admin)",
    roleVariant: "warning",
  },
];

interface DevCredentialsCheatsheetProps {
  activeEmail: string;
  onSelect: (credential: DevCredential) => void;
}

const badgeClass: Record<DevCredential["roleVariant"], string> = {
  danger:  "bg-danger  bg-opacity-25 text-danger  border border-danger",
  warning: "bg-warning bg-opacity-25 text-warning border border-warning",
  primary: "bg-primary bg-opacity-10 text-primary border border-primary",
};

const DevCredentialsCheatsheet = ({ activeEmail, onSelect }: DevCredentialsCheatsheetProps) => {
  if (!import.meta.env.DEV) return null;

  return (
    <div className="mt-4 w-100 border rounded overflow-hidden">
      <div className="d-flex align-items-center justify-content-between px-3 py-2 bg-light border-bottom">
        <div className="d-flex align-items-center gap-2">
          <Icon icon="solar:key-outline" className="text-warning" />
          <span className="fw-semibold small">Dev Credentials</span>
        </div>
        <span className="text-muted" style={{ fontSize: '0.75rem' }}>click row to fill</span>
      </div>
      <table className="w-100 small mb-0">
        <thead>
          <tr className="text-muted border-bottom">
            <th className="px-3 py-2 fw-medium text-start">Email</th>
            <th className="px-3 py-2 fw-medium text-start">Role</th>
          </tr>
        </thead>
        <tbody>
          {DEV_CREDENTIALS.map((credential) => (
            <tr
              key={credential.email}
              role="button"
              title={`Password: ${credential.password}`}
              onClick={() => onSelect(credential)}
              style={{ cursor: 'pointer' }}
              className={activeEmail === credential.email ? "bg-primary bg-opacity-10" : ""}
            >
              <td className="px-3 py-2">{credential.email}</td>
              <td className="px-3 py-2">
                <span className={`badge fw-medium ${badgeClass[credential.roleVariant]}`}>
                  {credential.role}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="px-3 py-2 bg-light border-top text-center text-muted" style={{ fontSize: '0.75rem' }}>
        Password shown in tooltip · Row highlights when filled
      </div>
    </div>
  );
};

export default DevCredentialsCheatsheet;
