import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const CategoriesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Categories' />
      <LookupSettingsLayer resourcePath='categories' title='Category' permissions={Permissions.Settings.Category} />
    </MasterLayout>
  );
};

export default CategoriesPage;
