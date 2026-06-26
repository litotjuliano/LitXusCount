import MasterLayout from "../masterLayout/MasterLayout";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const CustomerTypesPage = () => {
  return (
    <MasterLayout>
      <LookupSettingsLayer resourcePath='customer-types' title='Customer Type' permissions={Permissions.Settings.CustomerType} />
    </MasterLayout>
  );
};

export default CustomerTypesPage;
