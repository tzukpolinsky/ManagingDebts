import { Component, OnInit } from '@angular/core';
import { UploadFileService } from '../../services/upload-file.service';
import { Subject } from 'rxjs';
import { BillingSummary } from '../../entites/billing-summary';
import {
  takeUntil,
} from 'rxjs/operators';
import { Msg } from '../../entites/msg';
import { MatProgressButtonOptions } from 'mat-progress-buttons';
import { TableHeader } from '../../entites/table-headers';
import { TableBtn } from '../../entites/table-btn';
import { NgxSpinnerService } from 'ngx-spinner';
import { BillingSummaryService } from '../../services/billing-summary.service';
import Swal from 'sweetalert2';
import { User } from '../../entites/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html'
})
export class FileUploadComponent implements OnInit {
  file: File;
  destroyed = new Subject();
  msg: Msg;
  deleteBtn: TableBtn;
  dataHeaders: TableHeader[];
  summaries: BillingSummary[];
  isLoading: boolean;
  btnOpts: MatProgressButtonOptions = {
    active: false,
    text: 'טען',
    spinnerSize: 19,
    raised: false,
    stroked: true,
    flat: false,
    fab: false,
    fullWidth: false,
    disabled: false,
    mode: 'indeterminate',
    customClass: 'btn btn-fill btn-primary',
    // add an icon to the button
    // buttonIcon: {
    //   fontSet: 'fa',
    //   fontIcon: 'fa-heart',
    //   inline: true
    // }
  };
  constructor(private fileService: UploadFileService, private spinner: NgxSpinnerService,
              private billingSummaryService: BillingSummaryService, private userService: UserService) { }

  ngOnInit() {
    this.dataHeaders = this.billingSummaryService.getTableHeaders();
    this.deleteBtn = {
      type: 'danger',
      text: 'מחק מידע',
      condition: (row: any) => !JSON.parse(localStorage.getItem('user')).isSuperAdmin && row.isSent
    };
    this.getAllSummaries();
  }
  private getAllSummaries() {
    this.billingSummaryService.GetAllSummaryByCustomer()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => this.summaries = result);
  }
  uploadFile() {
    this.isLoading = true;
    this.spinner.show();
    if (this.file && this.file.size > 0 ) {
      this.fileService.uploadFile(this.file)
      .subscribe(result => {
        this.msg = {
          type: result ? 'success' : 'danger',
          text: result ? 'הקובץ נקלט בהצלחה' : 'קרתה תקלה בעת קליטת הקובץ',
          dismissible: true,
        };
        if (result) {
          this.getAllSummaries();
          this.file = null;
        }
        this.isLoading = false;
        this.spinner.hide();
      });
    }
  }
  onFileChange(event) {
    this.file = event.target.files[0];
    if (!this.fileService.checkFileTypeBySupplier(this.file)) {
      Swal.fire({
        title: 'לא ניתן להעלות את הקובץ',
        text: 'לא ניתן להעלות קובץ מסוג ' + this.file.type,
        icon: 'warning',
      });
      this.file = null;
    }
  }
  deleteRow(row) {
    this.isLoading = true;
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user.isSuperAdmin) {
      this.deleteDataBySummary(row);
    } else if (!row.isSent) {
      this.deleteDataBySummary(row);
    } else {
      user.isActive = false;
      this.userService.editUser(user)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.userService.logOut();
      });
    }
  }
  private deleteDataBySummary(row) {
    this.billingSummaryService.deleteDataBySummary(row)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.msg = {
        type: result ? 'success' : 'danger',
        text: result ? 'המידע נמחק בהצלחה' : 'קרתה תקלה בעת מחיקת המידע',
        dismissible: true,
      };
      if (result) {
        this.getAllSummaries();
      }
      this.isLoading = false;
    });
  }
}
