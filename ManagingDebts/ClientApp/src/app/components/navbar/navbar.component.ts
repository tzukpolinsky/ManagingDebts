import { Component, OnInit, ElementRef, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { MenuService } from '../../services/menu.service';
import { Menu } from '../../entites/menu';
import { Customer } from '../../entites/customer';
import { Supplier } from '../../entites/supplier';
import { CustomerService } from '../../services/customer.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { SupplierService } from '../../services/supplier.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit, OnDestroy {
  private listTitles: Menu[];
  location: Location;
  mobileMenuVisible: any = 0;
  customers: Customer[];
  suppliers: Supplier[];
  selectedCustomer: Customer;
  selectedSupplier: Supplier;
  destroyed = new Subject();
  private toggleButton: any;
  private sidebarVisible: boolean;

  public isCollapsed = true;

  closeResult: string;

  constructor(
    location: Location,
    private element: ElementRef,
    private router: Router,
    private modalService: NgbModal,
    private menuService: MenuService,
    private customerService: CustomerService,
    private supplierService: SupplierService,
    private userService: UserService
  ) {
    this.location = location;
    this.sidebarVisible = false;
  }
  // function that adds color white/transparent to the navbar on resize (this is for the collapse)
   updateColor = () => {
   const navbar = document.getElementsByClassName('navbar')[0];
   if (window.innerWidth < 993 && !this.isCollapsed) {
       navbar.classList.add('bg-white');
       navbar.classList.remove('navbar-transparent');
     } else {
       navbar.classList.remove('bg-white');
       navbar.classList.add('navbar-transparent');
     }
   }
  ngOnInit() {
    // localStorage.setItem('userId', '313537953');

    this.selectedCustomer = this.customerService.createEmptyEntity();
    this.selectedSupplier = this.supplierService.createEmptyEntity();
    window.addEventListener('resize', this.updateColor);
    const navbar: HTMLElement = this.element.nativeElement;
    this.toggleButton = navbar.getElementsByClassName('navbar-toggler')[0];
    this.router.events.subscribe(event => {
      this.sidebarClose();
      const $layer: any = document.getElementsByClassName('close-layer')[0];
      if ($layer) {
        $layer.remove();
        this.mobileMenuVisible = 0;
      }
    });

    this.customerService.getByUser()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      const sessionCustomer = localStorage.getItem('customerId');
      this.selectedCustomer = sessionCustomer ? result.find(x => x.id ===
         parseInt(sessionCustomer, 10)) : result[0];
      localStorage.setItem('customerId', this.selectedCustomer.id.toString());
      this.customers = result;
      this.getSuppliers();
    });
  }
  logOut() {
    this.userService.logOut();
  }
  customerChanged() {
    localStorage.setItem('customerId', this.selectedCustomer.id.toString());
    window.location.reload();
  }
  supplierChanged() {
    localStorage.setItem('supplier', JSON.stringify(this.selectedSupplier));
    window.location.reload();
  }
  private getSuppliers() {
    this.supplierService.getSupplierByCustomer()
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        const sessionSupplier = this.supplierService.getCurrentSupplier();
        this.selectedSupplier = sessionSupplier ? result.find(x => x.id === sessionSupplier.id)
        : result[0];
        localStorage.setItem('supplier', JSON.stringify(this.selectedSupplier));
        this.suppliers = result;
        this.listTitles = this.menuService.getMenu();
      });
  }
  collapse() {
    this.isCollapsed = !this.isCollapsed;
    const navbar = document.getElementsByTagName('nav')[0];
    if (!this.isCollapsed) {
      navbar.classList.remove('navbar-transparent');
      navbar.classList.add('bg-white');
    } else {
      navbar.classList.add('navbar-transparent');
      navbar.classList.remove('bg-white');
    }
  }

  sidebarOpen() {
    const toggleButton = this.toggleButton;
    const mainPanel =  (
      document.getElementsByClassName('main-panel')[0]
    ) as HTMLElement;
    const html = document.getElementsByTagName('html')[0];
    if (window.innerWidth < 991) {
      mainPanel.style.position = 'fixed';
    }

    setTimeout(() => {
      toggleButton.classList.add('toggled');
    }, 500);

    html.classList.add('nav-open');

    this.sidebarVisible = true;
  }
  sidebarClose() {
    const html = document.getElementsByTagName('html')[0];
    this.toggleButton.classList.remove('toggled');
    const mainPanel =  (
      document.getElementsByClassName('main-panel')[0]
    ) as HTMLElement;

    if (window.innerWidth < 991) {
      setTimeout(() => {
        mainPanel.style.position = '';
      }, 500);
    }
    this.sidebarVisible = false;
    html.classList.remove('nav-open');
  }
  sidebarToggle() {
    // const toggleButton = this.toggleButton;
    // const html = document.getElementsByTagName('html')[0];
    const $toggle = document.getElementsByClassName('navbar-toggler')[0];

    if (this.sidebarVisible === false) {
      this.sidebarOpen();
    } else {
      this.sidebarClose();
    }
    const html = document.getElementsByTagName('html')[0];

    if (this.mobileMenuVisible === 1) {
      // $('html').removeClass('nav-open');
      html.classList.remove('nav-open');
      // if ($layer) {
      //   $layer.remove();
      // }
      setTimeout(() => {
        $toggle.classList.remove('toggled');
      }, 400);

      this.mobileMenuVisible = 0;
    } else {
      setTimeout(() => {
        $toggle.classList.add('toggled');
      }, 430);

      const $layer = document.createElement('div');
      $layer.setAttribute('class', 'close-layer');

      if (html.querySelectorAll('.main-panel')) {
        document.getElementsByClassName('main-panel')[0].appendChild($layer);
      } else if (html.classList.contains('off-canvas-sidebar')) {
        document
          .getElementsByClassName('wrapper-full-page')[0]
          .appendChild($layer);
      }

      setTimeout(() => {
        $layer.classList.add('visible');
      }, 100);

      $layer.onclick = function() {
        // asign a function
        html.classList.remove('nav-open');
        this.mobileMenuVisible = 0;
        $layer.classList.remove('visible');
        setTimeout(() => {
          $layer.remove();
          $toggle.classList.remove('toggled');
        }, 400);
      }.bind(this);

      html.classList.add('nav-open');
      this.mobileMenuVisible = 1;
    }
  }

  getTitle() {
    let titlee = this.location.prepareExternalUrl(this.location.path());
    if (titlee.charAt(0) === '#') {
      titlee = titlee.slice(1);
    }
    for (let item = 0; item < this.listTitles.length; item++) {
      if (this.listTitles[item].Href && this.listTitles[item].Href === titlee) {
        return this.listTitles[item].Text;
      } else if (this.listTitles[item].SubMenus) {
        for (let index = 0; index < this.listTitles[item].SubMenus.length; index++) {
          if (this.listTitles[item].SubMenus[index].Href
            && this.listTitles[item].SubMenus[index].Href === titlee) {
            return this.listTitles[item].SubMenus[index].Text;
          }

        }
      }

    }
    return 'Dashboard';
  }

  open(content) {
    this.modalService.open(content, {windowClass: 'modal-search'}).result.then((result) => {
      this.closeResult = `Closed with: ${result}`;
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return  `with: ${reason}`;
    }
  }
  ngOnDestroy() {
     window.removeEventListener('resize', this.updateColor);
     this.destroyed.next();
  }
}
