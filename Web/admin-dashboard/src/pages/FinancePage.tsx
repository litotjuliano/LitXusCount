import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import PlaceholderModuleLayer from "../components/PlaceholderModuleLayer";

const FinancePage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Finance' />
      <PlaceholderModuleLayer moduleName='Finance' />
    </MasterLayout>
  );
};

export default FinancePage;
