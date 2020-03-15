import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AdminLayoutRoutes } from './admin-layout.routing';
import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
import {DataConfirmationComponent} from '../../pages/data-confirmation/data-confirmation.component';
import { FileUploadComponent } from '../../pages/file-upload/file-upload.component';
import { SliComponent } from '../../pages/sli/sli.component';
import { StatusPipe } from '../../pipes/status.pipe';
import { DatePipe } from '../../pipes/date.pipe';
import { ContractManagementComponent } from '../../pages/system/contract-management/contract-management.component';
import { BankManagementComponent } from '../../pages/system/bank-management/bank-management.component';
import { BudgetManagementComponent } from '../../pages/system/budget-management/budget-management.component';
import { ComponentsModule } from '../../components/components.module';
import { NgxDaterangepickerMd } from 'ngx-daterangepicker-material';
import { SweetAlert2Module} from '@sweetalert2/ngx-sweetalert2';
import { SupplierManagementComponent } from '../../pages/system/supplier-management/supplier-management.component';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { UserManagementComponent } from '../../pages/system/user-management/user-management.component';
import { SummaryGraphComponent } from '../../pages/dashboard/summary-graph/summary-graph.component';
import { ContractGraphComponent } from '../../pages/dashboard/contract-graph/contract-graph.component';
import { BudgetGraphComponent } from '../../pages/dashboard/budget-graph/budget-graph.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(AdminLayoutRoutes),
    HttpClientModule,
    NgxDaterangepickerMd.forRoot(),
    ComponentsModule, // a lot of the external components comes from there
    SweetAlert2Module.forRoot(),
    MatCheckboxModule,

  ],
  declarations: [
    DashboardComponent,
    StatusPipe,
    DatePipe,
    DataConfirmationComponent,
    FileUploadComponent,
    SliComponent,
    ContractManagementComponent,
    BankManagementComponent,
    BudgetManagementComponent,
    SupplierManagementComponent,
    UserManagementComponent,
    SummaryGraphComponent,
    ContractGraphComponent,
    BudgetGraphComponent,
  ]
})
export class AdminLayoutModule {}
