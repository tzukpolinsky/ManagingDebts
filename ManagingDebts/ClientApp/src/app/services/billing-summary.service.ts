import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BillingSummary } from '../entites/billing-summary';
import { Supplier } from '../entites/supplier';
import { SupplierService } from './supplier.service';
import { User } from '../entites/user';

@Injectable({
  providedIn: 'root'
})
export class BillingSummaryService {
  supplier = this.supplierService.getCurrentSupplier();
  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  createEmptyEntity(): BillingSummary {
    return  {
      customerId: this.supplier.customerId,
      supplierId: this.supplier.id,
      billingFrom: new Date(),
      billingTo: new Date(),
      totalFixed: 0,
      totalBilling: 0,
      totalBillingBeforeTax: 0,
      totalChangable: 0,
      totalCredit: 0,
      totalDebit: 0,
      totalOneTime: 0,
      rowId: 0,
      isSent: false,
      dateOfValue: new Date(),
      totalExtraPayments: 0,
      supplierClientNumber: 0,
    };
  }
  GetAllSummaryByCustomer() {
    const generelSummary = this.createEmptyEntity();
    const url = environment.serverUrl + 'uploadFile/getAllSummaryByCustomer';
    return this.httpClient.post<BillingSummary[]>(url, generelSummary);
  }
  deleteDataBySummary(row: BillingSummary) {
    const user: User = JSON.parse(localStorage.getItem('user'));
    const url = environment.serverUrl + 'uploadFile/deleteDataBySummary';
    if (user.isSuperAdmin) {
      return this.httpClient.post<boolean>(url, row);
    } else if (!row.isSent) {
      return this.httpClient.post<boolean>(url, row);
    }
  }
  getTableHeaders() {
    switch (this.supplier.id) {
      case '520031931':
        return this.getBezekTableHeaders();
      case '520000472':
        return this.getElectricityTableHeaders();
      default:
        this.getPrivateSupplierTableHeaders();
    }
  }
  private getBezekTableHeaders() {
    return [
      {
        id: 'billingFrom' ,
        name: 'תחילת חשבון',
        showColumn: false,
      },
      {
        id: 'billingTo' ,
        name: 'סוף חשבון',
        showColumn: false,
      },
      {
        id: 'dateOfValue' ,
        name: 'תאריך ערך',
        showColumn: true,
      },
      {
        id: 'totalFixed' ,
        name: 'תשלומים קבועים',
        showColumn: false,
      },
      {
        id: 'totalChangable' ,
        name: 'תשלומים משתנים',
        showColumn: false,
      },
      {
        id: 'totalOneTime' ,
        name: 'תשלומים חד פעמיים',
        showColumn: false,
      },
      {
        id: 'totalCredit' ,
        name: 'סה"כ זיכויים',
        showColumn: true,
      },
      {
        id: 'totalBilling' ,
        name: 'סה"כ חשבונית',
        showColumn: false,
      },
      {
        id: 'totalDebit' ,
        name: 'סה"כ שולם',
        showColumn: true,
      },
      {
        id: 'isSent' ,
        name: 'נשלח להנהלת חשבונות',
        showColumn: true,
      },
      {
        id: 'totalBillingBeforeTax' ,
        name: 'סה"כ לפני מע"מ',
        showColumn: false,
      },
    ];
  }
  private getElectricityTableHeaders() {
    return [
      {
        id: 'billingFrom' ,
        name: 'תחילת חשבון',
        showColumn: true,
      },
      {
        id: 'billingTo' ,
        name: 'סוף חשבון',
        showColumn: true,
      },
      {
        id: 'dateOfValue' ,
        name: 'תאריך ערך',
        showColumn: true,
      },
      {
        id: 'totalFixed' ,
        name: 'תשלומים קבועים',
        showColumn: false,
      },
      {
        id: 'totalChangable' ,
        name: 'תשלומים משתנים',
        showColumn: false,
      },
      {
        id: 'totalOneTime' ,
        name: 'תשלומים חד פעמיים',
        showColumn: false,
      },
      {
        id: 'totalCredit' ,
        name: 'סה"כ זיכויים',
        showColumn: false,
      },
      {
        id: 'totalBilling' ,
        name: 'סה"כ חשבונית',
        showColumn: false,
      },
      {
        id: 'totalDebit' ,
        name: 'סה"כ שולם',
        showColumn: true,
      },
      {
        id: 'isSent' ,
        name: 'נשלח להנהלת חשבונות',
        showColumn: true,
      },
      {
        id: 'totalBillingBeforeTax' ,
        name: 'סה"כ לפני מע"מ',
        showColumn: false,
      },
    ];
  }
  private getPrivateSupplierTableHeaders() {
    return [
      {
        id: 'billingFrom' ,
        name: 'תחילת חשבון',
        showColumn: true,
      },
      {
        id: 'billingTo' ,
        name: 'סוף חשבון',
        showColumn: true,
      },
      {
        id: 'dateOfValue' ,
        name: 'תאריך ערך',
        showColumn: true,
      },
      {
        id: 'totalFixed' ,
        name: 'תשלומים קבועים',
        showColumn: false,
      },
      {
        id: 'totalChangable' ,
        name: 'תשלומים משתנים',
        showColumn: false,
      },
      {
        id: 'totalOneTime' ,
        name: 'תשלומים חד פעמיים',
        showColumn: false,
      },
      {
        id: 'totalCredit' ,
        name: 'סה"כ זיכויים',
        showColumn: false,
      },
      {
        id: 'TotalInvoice' ,
        name: 'סה"כ חשבונית',
        showColumn: false,
      },
      {
        id: 'totalDebit' ,
        name: 'סה"כ שולם',
        showColumn: true,
      },
      {
        id: 'isSent' ,
        name: 'נשלח להנהלת חשבונות',
        showColumn: true,
      },
      {
        id: 'totalBillingBeforeTax' ,
        name: 'סה"כ לפני מע"מ',
        showColumn: false,
      },
    ];
  }
}
