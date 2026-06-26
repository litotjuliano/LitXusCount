import MasterLayout from "../masterLayout/MasterLayout";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const PaymentTypesPage = () => {
  return (
    <MasterLayout>
      <LookupSettingsLayer resourcePath='payment-types' title='Payment Type' permissions={Permissions.Settings.PaymentType} />
    </MasterLayout>
  );
};

export default PaymentTypesPage;
