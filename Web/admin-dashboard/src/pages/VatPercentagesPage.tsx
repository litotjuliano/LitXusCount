import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import VatPercentageSettingsLayer from "../components/settings/VatPercentageSettingsLayer";

const VatPercentagesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='VAT Percentage' />
      <VatPercentageSettingsLayer />
    </MasterLayout>
  );
};

export default VatPercentagesPage;
