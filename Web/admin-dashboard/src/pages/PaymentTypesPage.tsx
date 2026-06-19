import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const PaymentTypesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Payment Type' />
      <LookupSettingsLayer resourcePath='payment-types' title='Payment Type' permissions={Permissions.Settings.PaymentType} />
    </MasterLayout>
  );
};

export default PaymentTypesPage;
