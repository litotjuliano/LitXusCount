import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import PlaceholderModuleLayer from "../components/PlaceholderModuleLayer";

const SalesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Sales' />
      <PlaceholderModuleLayer moduleName='Sales' />
    </MasterLayout>
  );
};

export default SalesPage;
