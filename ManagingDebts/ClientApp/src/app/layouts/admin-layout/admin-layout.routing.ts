import { Routes } from '@angular/router';

import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
// import { RtlComponent } from "../../pages/rtl/rtl.component";
import { DataConfirmationComponent } from '../../pages/data-confirmation/data-confirmation.component';
import { SliComponent } from '../../pages/sli/sli.component';
import { FileUploadComponent } from '../../pages/file-upload/file-upload.component';

import { ContractManagementComponent } from '../../pages/system/contract-management/contract-management.component';
// tslint:disable-next-line:max-line-length
import { BudgetManagementComponent } from '../../pages/system/budget-management/budget-management.component';
import { BankManagementComponent } from '../../pages/system/bank-management/bank-management.component';
import { SupplierManagementComponent } from '../../pages/system/supplier-management/supplier-management.component';
import { UserManagementComponent } from '../../pages/system/user-management/user-management.component';
export const AdminLayoutRoutes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'dataConfirmation', component: DataConfirmationComponent  },
  { path: 'Sli', component: SliComponent  },
  { path: 'fileUpload', component: FileUploadComponent  },
  { path: 'userManagement/Contracts', component: ContractManagementComponent  },
  { path: 'userManagement/Banks', component: BankManagementComponent  },
  { path: 'userManagement/Budgets', component: BudgetManagementComponent  },
  { path: 'userManagement/Supplier', component: SupplierManagementComponent  },
  { path: 'userManagement/Users', component: UserManagementComponent  },
  // { path: "rtl", component: RtlComponent }
];
