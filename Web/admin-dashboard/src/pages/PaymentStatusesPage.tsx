import MasterLayout from "../masterLayout/MasterLayout";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const PaymentStatusesPage = () => {
  return (
    <MasterLayout>
      <LookupSettingsLayer resourcePath='payment-statuses' title='Payment Status' permissions={Permissions.Settings.PaymentStatus} />
    </MasterLayout>
  );
};

export default PaymentStatusesPage;
