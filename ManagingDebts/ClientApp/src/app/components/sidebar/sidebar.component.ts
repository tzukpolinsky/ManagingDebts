import { Component, OnInit } from '@angular/core';
import { Menu } from '../../entites/menu';
import { MenuService } from '../../services/menu.service';
import { User } from '../../entites/user';
import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  menuItems: Menu[];
  selectedMenuItem: string;
  user: User;
  private menuOpen = 'tim-icons icon-minimal-up';
  private menuClose = 'tim-icons icon-minimal-down';
  constructor(private menuService: MenuService, private supplierService: SupplierService) {}
  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('user'));
    if (this.supplierService.getCurrentSupplier()) {
      this.menuItems = this.menuService.getMenu();
      this.selectedMenuItem = this.selectedMenuItem ?
      this.menuItems.find(x => x.Text === this.selectedMenuItem).Text : this.menuItems[0].Text;
    }
  }
  isMobileMenu() {
    if (window.innerWidth > 991) {
      return false;
    }
    return true;
  }
  openSubMenu($event, menuItem: Menu) {
  if (menuItem.SubMenus) {
  if (this.selectedMenuItem === menuItem.Text) {
    $event.currentTarget.children[0].children[1].children[0].className = this.menuClose;
    this.selectedMenuItem = '';
  } else {
    $event.currentTarget.children[0].children[1].children[0].className = this.menuOpen;
    this.selectedMenuItem = menuItem.Text;
  }
  } else {
    const menusWithSubMenusIcons = document.querySelectorAll('.hasSubMenus>a>p>i');
    menusWithSubMenusIcons.forEach(x => x.className = this.menuClose);
    this.selectedMenuItem = menuItem.Text;
  }


}
}
