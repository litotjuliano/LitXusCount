import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import EmailConfigSettingsLayer from "../components/settings/EmailConfigSettingsLayer";

const EmailConfigsPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Email Config' />
      <EmailConfigSettingsLayer />
    </MasterLayout>
  );
};

export default EmailConfigsPage;
