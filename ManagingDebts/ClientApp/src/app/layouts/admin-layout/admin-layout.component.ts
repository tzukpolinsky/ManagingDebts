import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../../entites/user';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Msg } from '../../entites/msg';
import { CustomerService } from '../../services/customer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { MenuService } from '../../services/menu.service';
import { SendMailService } from '../../services/send-mail.service';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  public sidebarColor = 'red';
  destroyed = new Subject();
  user: User;
  isAuth: boolean;
  isLoading: boolean;
  msg: Msg;
  Image = '/assets/img/background.jpg';
  base64Image = '';
  constructor(private userService: UserService, private spinner: NgxSpinnerService, private menuService: MenuService,
              private mailService: SendMailService) {}
  ngOnInit() {
    addEventListener('storage', (event) => {
      this.handleLocalStorageChange(event);
    });
    this.insertImageToLocaleStorage();
    this.checkAuth();
  }
  handleLocalStorageChange(event: StorageEvent) {
    try {
      const user: User = JSON.parse(event.oldValue);
      user.isSuperAdmin = false;
      user.isActive = false;
      this.mailService.reportManualChangeInLocaleStorage(user)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
      });
      this.userService.editUser(user)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.userService.logOut();
      });
    } catch (error) {
      this.userService.logOut();
      localStorage.clear();
    }
  }
  insertImageToLocaleStorage() {
    this.base64Image = localStorage.getItem('bg-image');
    if (!this.base64Image) {
      this.menuService.getBgImg().subscribe(result => {
        const reader = new FileReader();
        reader.readAsDataURL(result);
        reader.onloadend = () => {
            localStorage.setItem('bg-image', reader.result.toString());
        };
      });
    } else {
      this.Image = this.base64Image;
    }
  }
  checkAuth() {
    try {
      this.user = this.userService.createEmptyEntity();
      const currentUser = localStorage.getItem('user');
      if (currentUser) {
        const user: User = JSON.parse(currentUser);
        this.isAuth = user.isActive;
      }
    } catch (error) {
      this.userService.logOut();
      localStorage.clear();
    }
  }
  ngOnDestroy() {
    this.destroyed.next();
  }
  changeSidebarColor(color) {
    const sidebar = document.getElementsByClassName('sidebar')[0];
    const mainPanel = document.getElementsByClassName('main-panel')[0];

    this.sidebarColor = color;

    if (sidebar !== undefined) {
        sidebar.setAttribute('data', color);
    }
    if (mainPanel !== undefined) {
        mainPanel.setAttribute('data', color);
    }
  }
  changeDashboardColor(color) {
    const body = document.getElementsByTagName('body')[0];
    if (body && color === 'white-content') {
        body.classList.add(color);
    } else if (body.classList.contains('white-content')) {
      body.classList.remove('white-content');
    }
  }

  login() {
    this.isLoading = true;
    this.spinner.show();
    this.userService.login(JSON.parse(JSON.stringify(this.user)))// create a deep copy
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      if (result.isSuccess) {
        localStorage.setItem('user', JSON.stringify(result.user));
        localStorage.setItem('customerId', result.user.customerId);
        this.isAuth = true;
      }
      this.msg = {
        text: result.msg,
        type: result.isSuccess ? 'success' : 'danger',
        dismissible: false
      };
      this.isLoading = false;
      this.spinner.hide();
    });
  }
}
