import { Menu } from '../entites/menu';
import { Supplier } from '../entites/supplier';
import { User } from '../entites/user';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SupplierService } from './supplier.service';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  getMenu(): Array<Menu> {
    const supplier: Supplier = this.supplierService.getCurrentSupplier();
    const user: User = JSON.parse(localStorage.getItem('user'));
    return [
      { Text: 'העלת קובץ', Href: '/fileUpload' , icon: 'icon-upload'},
      {
        Text: 'דוחות',
        Href: '/dashboard',
        icon: 'icon-chart-pie-36',
      },
      { Text: 'בדיקת נתונים', Href: '/dataConfirmation', icon: 'icon-bank' },
      { Text: 'הנהלת חשבונות', Href: '/Sli', icon: 'icon-book-bookmark' },
      {
        Text: 'מערכת',
        SubMenus: [
          { Text: 'חוזים', Href: '/userManagement/Contracts' },
          supplier && supplier.withBanks ? { Text: 'בנקים', Href: '/userManagement/Banks' } : null,
          { Text: 'סעיפים תקציביים', Href: '/userManagement/Budgets' },
          { Text: 'ספקים', Href: '/userManagement/Supplier' },
          user.isSuperAdmin ? {Text: 'משתמשים', Href: '/userManagement/Users'} : null,
        ]
        , icon: 'icon-single-02'
      }
    ];
  }
  getBgImg() {
    return this.httpClient.get<any>('/assets/img/background.jpg', { observe: 'body', responseType: 'blob' as 'json' });
  }
}
