import MasterLayout from "../masterLayout/MasterLayout";
import Breadcrumb from "../components/Breadcrumb";
import UserManagementLayer from "../components/admin/UserManagementLayer";

const UsersPage = () => {
  return (
    <MasterLayout>
      <Breadcrumb title='Users' />
      <UserManagementLayer />
    </MasterLayout>
  );
};

export default UsersPage;
