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

const roleBadgeClass: Record<DevCredential["roleVariant"], string> = {
  danger: "bg-danger-focus text-danger-main border border-danger-main",
  warning: "bg-warning-focus text-warning-main border border-warning-main",
  primary: "bg-primary-50 text-primary-600 border border-primary-600",
};

const DevCredentialsCheatsheet = ({ activeEmail, onSelect }: DevCredentialsCheatsheetProps) => {
  if (!import.meta.env.DEV) {
    return null;
  }

  return (
    <div className='mt-32 max-w-464-px mx-auto w-100 border radius-12 overflow-hidden'>
      <div className='d-flex align-items-center justify-content-between px-16 py-12 bg-neutral-50 border-bottom'>
        <div className='d-flex align-items-center gap-2'>
          <Icon icon='solar:key-outline' className='text-warning-main' />
          <span className='fw-semibold text-sm'>Dev Credentials</span>
        </div>
        <span className='text-xs text-secondary-light'>click row to fill</span>
      </div>
      <table className='w-100 text-sm'>
        <thead>
          <tr className='text-secondary-light'>
            <th className='text-start px-16 py-8 fw-medium'>Email</th>
            <th className='text-start px-16 py-8 fw-medium'>Role</th>
          </tr>
        </thead>
        <tbody>
          {DEV_CREDENTIALS.map((credential) => (
            <tr
              key={credential.email}
              role='button'
              title={`Password: ${credential.password}`}
              onClick={() => onSelect(credential)}
              className={
                activeEmail === credential.email
                  ? "bg-primary-50 cursor-pointer"
                  : "cursor-pointer hover-bg-neutral-50"
              }
            >
              <td className='px-16 py-10'>{credential.email}</td>
              <td className='px-16 py-10'>
                <span className={`px-8 py-2 radius-8 text-xs fw-medium ${roleBadgeClass[credential.roleVariant]}`}>
                  {credential.role}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className='px-16 py-8 bg-neutral-50 border-top text-xs text-secondary-light text-center'>
        Password shown in tooltip · Row highlights when filled
      </div>
    </div>
  );
};

export default DevCredentialsCheatsheet;
