import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BillingSummary } from '../entites/billing-summary';
import { environment } from '../../environments/environment';
import { SupplierService } from './supplier.service';

@Injectable({
  providedIn: 'root'
})
export class DataConfirmationService {
  supplier = this.supplierService.getCurrentSupplier();

  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  getTableHeaders() {
    switch (this.supplier.id) {
      case '520031931':
        return this.getBezekTableHeaders();
      case '520000472':
        return this.getElectricityTableHeaders();
      default:
        return this.getPrivateSupplierTableHeaders();
    }
  }
  getBezekTableHeaders() {
    return [
    {
      id: 'clientNumber',
      name: 'מספר לקוח בבזק',
      showColumn: false,
    },
    {
      id: 'payerNumberBezek',
      name: 'מספר משלם בבזק',
      showColumn: false,
    },
    {
      id: 'departmentNumber',
      name: 'מספר מחלקה',
      showColumn: false,
    },
    {
      id: 'contract',
      name: 'חוזה',
      showColumn: true,
    },
    {
      id: 'startDate',
      name: 'התחלת חיוב',
      showColumn: true,
    },
    {
      id: 'endDate',
      name: 'סיום חיוב',
      showColumn: true,
    },
    {
      id: 'type',
      name: 'סוג חיוב',
      showColumn: true,
    },
    {
      id: 'description',
      name: 'תיאור חיוב',
      showColumn: true,
    },
    {
      id: 'amountAfterTax',
      name: 'סה"כ',
      showColumn: true,
    },
    {
      id: 'amount',
      name: 'סה"כ לפני מס',
      showColumn: false,
    },
    {
      id: 'consumptionAmount',
      name: 'סה"כ צריכה',
      showColumn: false,
    },
    {
      id: 'monthlyRate',
      name: 'תעריף חודשי',
      showColumn: false,
    },
  ];
  }
  getElectricityTableHeaders() {
    return [
    {
      id: 'bank',
      name: 'מספר הבנק',
      showColumn: false,
    },
    {
      id: 'branch',
      name: 'מספר הסניף',
      showColumn: false,
    },
    {
      id: 'billCreatingDate',
      name: 'תאריך הפקת החשבון',
      showColumn: true,
    },
    {
      id: 'paymentDate',
      name: 'סיום חיוב',
      showColumn: true,
    },
    {
      id: 'numberOfCreditDays',
      name: 'מספר ימי אשראי',
      showColumn: false,
    },
    {
      id: 'amountAfterTax',
      name: 'סכום',
      showColumn: true,
    },
    {
      id: 'consumerAddress',
      name: 'כתובת צרכן',
      showColumn: false,
    },
    {
      id: 'consumerName',
      name: 'שם צרכן',
      showColumn: true,
    },
    {
      id: 'bankAccount',
      name: 'מספר חשבון בנק',
      showColumn: false,
    },
    {
      id: 'bankAccountType',
      name: 'סוג חשבון בנק',
      showColumn: false,
    },
    {
      id: 'monthOfLastInvoice',
      name: 'חודש של חשבונית אחרונה',
      showColumn: false,
    },
    {
      id: 'yearOfLastInvoice',
      name: 'שנה של חשבונית אחרונה',
      showColumn: false,
    },
    {
      id: 'consumerNumber',
      name: 'מספר צרכן',
      showColumn: false,
    },
    {
      id: 'contract',
      name: 'חוזה',
      showColumn: true,
    },
    {
      id: 'invoice',
      name: 'מספר חשבונית',
      showColumn: true,
    },
  ];
  }
  getPrivateSupplierTableHeaders() {
    return [
    {
      id: 'amountAfterTax',
      name: 'סכום אחרי מע"מ',
      showColumn: true,
    },
    {
      id: 'amount',
      name: 'סכום',
      showColumn: false,
    },
    {
      id: 'taxRate',
      name: 'אחוז המע"מ',
      showColumn: false,
    },
    {
      id: 'contract',
      name: 'חוזה',
      showColumn: true,
    },
    {
      id: 'dateOfValue',
      name: 'תאריך ערך',
      showColumn: true,
    },
    {
      id: 'invoice',
      name: 'מספר חשבונית',
      showColumn: true,
    },
    {
      id: 'description',
      name: 'תיאור',
      showColumn: true,
    },
  ];
  }
  match(data: any[]) {
    const dataToSent = [];
    data.forEach((row) => {
      row.data.forEach((dataRow) => {
        dataToSent.push(dataRow);
      });
    });
    let url = environment.serverUrl + 'dataConfirmation/';
    switch (this.supplier.id) {
      case '520031931':
        url += 'matchBezek';
        return this.httpClient.post<boolean>(url, dataToSent);
      case '520000472':
        url += 'matchElectricity';
        return this.httpClient.post<boolean>(url, dataToSent);
      default:
        url += 'matchPrivateSupplier';
        return this.httpClient.post<boolean>(url, dataToSent);
    }
  }
  getDataBySummary(summary: BillingSummary) {
    const url = environment.serverUrl + 'dataConfirmation/getDataBySummary';
    return this.httpClient.post<any[]>(url, summary);
  }
}
