import MasterLayout from "../masterLayout/MasterLayout";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const CategoriesPage = () => {
  return (
    <MasterLayout>
      <LookupSettingsLayer resourcePath='categories' title='Category' permissions={Permissions.Settings.Category} />
    </MasterLayout>
  );
};

export default CategoriesPage;
