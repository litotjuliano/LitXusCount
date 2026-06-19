import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import CompanyInfoSettingsLayer from "../components/settings/CompanyInfoSettingsLayer";

const CompanyInfoPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Company Info' />
      <CompanyInfoSettingsLayer />
    </MasterLayout>
  );
};

export default CompanyInfoPage;
