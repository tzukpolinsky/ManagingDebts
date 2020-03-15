import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Customer } from '../entites/customer';
import { environment } from '../../environments/environment';
import { Supplier } from '../entites/supplier';
import { TableHeader } from '../entites/table-headers';
import { CustomerService } from './customer.service';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {

  constructor(private httpClient: HttpClient, private customerService: CustomerService) { }
  createEmptyEntity(): Supplier {
    return {
      id: '',
      name: '',
      customerId: parseInt(localStorage.getItem('customerId'), 10),
      isEnable: true,
      supplierNumberInFinance: 6000000000,
      withBanks: false,
    };
  }
  createTableHeaders(): TableHeader[] {
    return [
      {
        id: 'supplierNumberInFinance',
        name: 'מזהה ספק בפיננסים',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'id',
        name: 'מזהה ספק',
        showColumn: false,
        isAsc: true,
      },
      {
        id: 'name',
        name: 'שם ספק',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'isPrivate',
        name: 'האם הספק פרטי',
        showColumn: false,
        isAsc: true,
      },
      {
        id: 'isEnable',
        name: 'האם הספק פעיל',
        showColumn: false,
        isAsc: true,
      },
      {
        id: 'withBanks',
        name: 'האם קיים ניהול בנקים',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'pkudatYomanNumber',
        name: 'מספר פקודת יומן אחרון',
        showColumn: true,
        isAsc: true,
      },
    ];
  }
  checkId(id: number) {
        const idStr = id.toString().padStart(9, '0');
        const oneTwo = [ 1, 2, 1, 2, 1, 2, 1, 2, 1 ];
        const multiply = [];
        const oneDigit = [];
        for (let i = 0; i < 9; i++) {
          multiply[i] = parseInt(idStr[i], 10) * oneTwo[i];
         }
        for (let i = 0; i < 9; i++) {
            oneDigit[i] = multiply[i] % 10;
            oneDigit[i] += multiply[i] >= 10 ? 1 : 0;
        }
        let sum = 0;
        for (let i = 0; i < 9; i++) {
          sum += oneDigit[i];
        }
        if (sum % 10 === 0) {
              return true;
         } else {
              return false;
         }
  }
  getSupplierByCustomer() {
    const customer: Customer = this.customerService.createEmptyEntity();
    customer.id = parseInt(localStorage.getItem('customerId'), 10);
    const url = environment.serverUrl + 'supplier/getSuppliersByCustomer';
    return this.httpClient.post<Supplier[]>(url, customer);
  }
  getCurrentSupplier(): Supplier {
    try {
      const supplier: Supplier = JSON.parse(localStorage.getItem('supplier'));
      if (supplier) {
        return supplier;
      } else {
        window.location.reload();
      }
    } catch (error) {
      // if we opening the application with an empty local storage
      window.location.reload();
    }
  }
  addSupplier(supplier: Supplier) {
    supplier.supplierNumberInFinance = parseInt(supplier.supplierNumberInFinance.toString(), 10);
    const url = environment.serverUrl + 'supplier/addSupplier';
    return this.httpClient.post<Supplier[]>(url, supplier);
  }
  editSupplier(supplier: Supplier) {
    supplier.customerId = parseInt(localStorage.getItem('customerId'), 10);
    const url = environment.serverUrl + 'supplier/editSupplier';
    return this.httpClient.post<Supplier[]>(url, supplier);
  }
}
