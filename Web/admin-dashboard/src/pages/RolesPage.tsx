import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import RoleManagementLayer from "../components/admin/RoleManagementLayer";

const RolesPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Roles' />
      <RoleManagementLayer />
    </MasterLayout>
  );
};

export default RolesPage;
