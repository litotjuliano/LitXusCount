import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const UnitsOfMeasurePage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Units of Measure' />
      <LookupSettingsLayer resourcePath='units-of-measure' title='Unit of Measure' permissions={Permissions.Settings.UnitOfMeasure} />
    </MasterLayout>
  );
};

export default UnitsOfMeasurePage;
