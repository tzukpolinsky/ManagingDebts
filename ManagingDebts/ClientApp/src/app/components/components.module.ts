import {
  NgModule
} from '@angular/core';
import {
  CommonModule
} from '@angular/common';
import {
  RouterModule
} from '@angular/router';
import {
  NgbModule
} from '@ng-bootstrap/ng-bootstrap';
import {
  NgMultiSelectDropDownModule
} from 'ng-multiselect-dropdown';

import {
  FooterComponent
} from './footer/footer.component';
import {
  NavbarComponent
} from './navbar/navbar.component';
import {
  SidebarComponent
} from './sidebar/sidebar.component';
import {
  NgxPaginationModule
} from 'ngx-pagination';
import {
  MsgComponent
} from './shared/msg/msg.component';

import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { DateStringPipe } from '../pipes/date-string.pipe';
import { MatProgressButtonsModule } from 'mat-progress-buttons';
import { TableComponent } from './shared/table/table.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { IsPipe } from '../pipes/is.pipe';
import { MailErrorComponent } from './shared/mail-error/mail-error.component';
@NgModule({
  imports: [CommonModule,
    RouterModule, NgbModule, FormsModule,
     NgxPaginationModule, NgMultiSelectDropDownModule.forRoot(),
     NgSelectModule, MatProgressButtonsModule, NgxSpinnerModule
    ],
  declarations: [FooterComponent,
    NavbarComponent, SidebarComponent,
    MsgComponent,
     DateStringPipe, TableComponent, IsPipe, MailErrorComponent, ],
  exports: [FooterComponent, NgbModule, MatProgressButtonsModule,
     NavbarComponent, SidebarComponent, TableComponent,
    MsgComponent,  DateStringPipe, NgxSpinnerModule,
    NgxPaginationModule, FormsModule, NgMultiSelectDropDownModule, NgSelectModule, IsPipe ]
})
export class ComponentsModule {}
