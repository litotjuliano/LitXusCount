import MasterLayout from "../masterLayout/MasterLayout";
import LookupSettingsLayer from "../components/settings/LookupSettingsLayer";
import { Permissions } from "../api/permissions";

const UnitsOfMeasurePage = () => {
  return (
    <MasterLayout>
      <LookupSettingsLayer resourcePath='units-of-measure' title='Unit of Measure' permissions={Permissions.Settings.UnitOfMeasure} />
    </MasterLayout>
  );
};

export default UnitsOfMeasurePage;
