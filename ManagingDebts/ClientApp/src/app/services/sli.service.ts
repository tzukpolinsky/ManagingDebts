import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../entites/user';
import { BillingSummary } from '../entites/billing-summary';
import { environment } from '../../environments/environment';
import { TableHeader } from '../entites/table-headers';

@Injectable({
  providedIn: 'root'
})
export class SliService {

  constructor(private htttpClient: HttpClient) { }
  createTableHeaders(): TableHeader[] {
    return [
      {
        id: 'taarichErech',
        name: 'תאריך ערך',
        showColumn: true,
      },
      {
        id: 'misparCheshbon',
        name: 'סעיף תקציבי',
        showColumn: true,
      },
      {
        id: 'teur',
        name: 'תיאור',
        showColumn: true,
      },
      {
        id: 'misparCheshbonit',
        name: 'חשבונית',
        showColumn: true,
      },
      {
        id: 'taarichCheshbonit',
        name: 'תאריך חשבונית',
        showColumn: false,
      },
      {
        id: 'misparCheshbonNegdi',
        name: 'מספר חשבון נגדי',
        showColumn: true,
      },
      {
        id: 'schum',
        name: 'סכום',
        showColumn: true,
      },
      {
        id: 'zchutChovaInd',
        name: 'ז/ח',
        showColumn: true,
      },
    ];
  }
  createSli(summary: BillingSummary, dateOfRegistration: Date, user: User) {
    const url = environment.serverUrl + 'Sli/createSli';
    return this.htttpClient.post<any>(url, {summary, dateOfRegistration, user});
  }
  displaySli(summary: BillingSummary, dateOfRegistration: Date) {
    const url = environment.serverUrl + 'Sli/displaySli';
    return this.htttpClient.post<any>(url, {summary, dateOfRegistration});
  }
}
