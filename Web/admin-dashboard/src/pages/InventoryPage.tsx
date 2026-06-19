import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import PlaceholderModuleLayer from "../components/PlaceholderModuleLayer";

const InventoryPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Inventory' />
      <PlaceholderModuleLayer moduleName='Inventory' />
    </MasterLayout>
  );
};

export default InventoryPage;
