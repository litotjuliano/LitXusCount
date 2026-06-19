import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import CurrencySettingsLayer from "../components/settings/CurrencySettingsLayer";

const CurrenciesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Manage Currency' />
      <CurrencySettingsLayer />
    </MasterLayout>
  );
};

export default CurrenciesPage;
