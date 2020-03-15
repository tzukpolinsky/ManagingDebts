import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { User } from '../../../entites/user';
import { UserService } from '../../../services/user.service';
import { takeUntil, take } from 'rxjs/operators';
import { Msg } from '../../../entites/msg';
import { NgxSpinnerService } from 'ngx-spinner';
import { Customer } from '../../../entites/customer';
import { CustomerService } from '../../../services/customer.service';
import { TableBtn } from '../../../entites/table-btn';
import { TableHeader } from '../../../entites/table-headers';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
})
export class UserManagementComponent implements OnInit, OnDestroy {
  destroyed = new Subject();
  newUser: User;
  userForUpdate: User;
  userForChange: User;
  currentUser = JSON.parse(localStorage.getItem('user'));
  users: User[];
  tableHeaders: TableHeader[];
  editBtn: TableBtn;
  changeBtn: TableBtn;
  customers: Customer[];
  isLoading: boolean;
  msg: Msg;
  editMsg: Msg;
  @ViewChild('changePasswordPopUp', {static: true }) private changePasswordPopUp: SwalComponent;
  @ViewChild('updateUserPopUp', {static: true }) private updateUserPopUp: SwalComponent;

  constructor(private userService: UserService, public readonly swalTargets: SwalPortalTargets,
              private spinner: NgxSpinnerService, private customerService: CustomerService) { }
  ngOnDestroy() {
    this.destroyed.next();
  }
  ngOnInit() {
    const user = JSON.parse(localStorage.getItem('user'));
    if (!user.isSuperAdmin) {
      this.userService.logOut();
    }
    this.tableHeaders = this.userService.createTableHeaders();
    this.newUser = this.userService.createEmptyEntity();
    this.userForUpdate = this.userService.createEmptyEntity();
    this.userService.getAll()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => this.users = result);
    this.customerService.getAll()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result =>
      this.customers = result);
    this.editBtn = {
      text: 'ערוך',
      type: 'warning',
    };
    this.changeBtn = {
      text: 'שנה סיסמא',
      type: 'danger',
    };
  }
  createUser() {
    if (!this.users.find(x => x.id === this.newUser.id)) {
      this.isLoading = true;
      this.spinner.show();
      this.userService.createUser(this.newUser, this.newUser.password)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'המשתמש נוצר בהצלחה' : 'קרתה תקלה בעת יצירת המשתמש',
          type: result ? 'success' : 'danger',
          dismissible: true,
        };
        this.isLoading = false;
        this.spinner.hide();
      });
    } else {
      this.msg = {
        text: 'המשתמש כבר קיים',
        type: 'danger',
        dismissible: false
      };
    }
  }
  popUpEditWindow(user) {
    this.userForUpdate = user;
    this.updateUserPopUp.fire();
  }
  popUpChangeWindow(user) {
    this.userForChange = user;
    this.changePasswordPopUp.fire();
  }
  changePassword() {
    this.isLoading = true;
    this.spinner.show();
    this.userService.changePassword([this.currentUser, this.userForChange])
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.msg = {
        text: result.msg,
        type: result.isSuccess ? 'success' : 'danger',
        dismissible: true
      };
      this.isLoading = false;
      this.spinner.hide();
    });
  }
  editUser() {
    this.isLoading = true;
    this.spinner.show();
    this.userService.editUser(this.userForUpdate)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.msg = {
        text: result ? 'עריכת המשתמש בוצעה בהצלחה' : 'קרתה תקלה בעת עריכת המשתמש',
        type: result ? 'success' : 'danger',
        dismissible: true
      };
      this.isLoading = false;
      this.spinner.hide();
    });
  }
  clearEditUser() {
    this.userForUpdate = null;
  }
  clearChangeUser() {
    this.userForUpdate = null;
  }
}
