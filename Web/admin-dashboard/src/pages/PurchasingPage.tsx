import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import PlaceholderModuleLayer from "../components/PlaceholderModuleLayer";

const PurchasingPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Purchasing' />
      <PlaceholderModuleLayer moduleName='Purchasing' />
    </MasterLayout>
  );
};

export default PurchasingPage;
