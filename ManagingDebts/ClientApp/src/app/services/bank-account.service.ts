import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import {BankAccount} from '../entites/bank-account';
import { TableHeader } from '../entites/table-headers';
import { Supplier } from '../entites/supplier';
import { SupplierService } from './supplier.service';
@Injectable({
  providedIn: 'root'
})
export class BankAccountService {
  supplier = this.supplierService.getCurrentSupplier();
  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  createEmptyEntity(): BankAccount {
    return {
      bankAccountInFinance: 0,
      customerId: this.supplier.customerId,
      supplierId : this.supplier.id
    };

  }
  getBankTableHeaders(): TableHeader[] {
    return [
      {
        id: 'bankAccountInFinance',
        name: 'מספר חשבון בפיננסים',
        showColumn: true,
      },
    ];
  }
  getBankAndBranchesName() {
    return require('../../assets/data/banks.json');
  }
  getBySupplier() {
    const url = environment.serverUrl + 'bankAccounts/getBySupplier';
    return this.httpClient.post<BankAccount[]>(url, this.supplier);
  }
  addBank(bank: BankAccount) {
    const url = environment.serverUrl + 'bankAccounts/addBank';
    return this.httpClient.post<BankAccount[]>(url, bank);
  }
  deleteBank(bank: BankAccount) {
    const url = environment.serverUrl + 'bankAccounts/deleteBank';
    return this.httpClient.post<BankAccount[]>(url, bank);
  }
}
