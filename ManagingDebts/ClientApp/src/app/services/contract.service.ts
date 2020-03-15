import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Contract } from '../entites/contract';
import { environment } from '../../environments/environment';
import { TableHeader } from '../entites/table-headers';
import { Supplier } from '../entites/supplier';
import { SupplierService } from './supplier.service';
@Injectable({
  providedIn: 'root'
})
export class ContractService {
  supplier = this.supplierService.getCurrentSupplier();
  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  createEmptyEntity(): Contract {
    return {
      id: 0,
      customerId: this.supplier.customerId,
      supplierId: this.supplier.id,
      description: '',
      bankAccountInFinance: 0,
      budgetContract: [],
    };
  }
  getContractTableHeaders(): TableHeader[] {
    if (this.supplier.withBanks) {
    return [
      {
        id: 'id',
        name: 'מזהה חוזה',
        showColumn: true,
      },
      {
        id: 'description',
        name: 'תיאור החוזה',
        showColumn: true,
      },
      {
        id: 'bankAccountInFinance',
        name: 'חשבון הבנק המקושר לחוזה',
        showColumn: true,
      },
      {
        id: 'budgetIds',
        name: 'סעיפים תקציביים',
        showColumn: true,
      },
      {
        id: 'address',
        name: 'כתובת החוזה',
        showColumn: false,
      },
    ];
  } else {
    return [
      {
        id: 'id',
        name: 'מזהה חוזה',
        showColumn: true,
      },
      {
        id: 'description',
        name: 'תיאור החוזה',
        showColumn: true,
      },
      {
        id: 'budgetIds',
        name: 'סעיפים תקציביים',
        showColumn: true,
      },
      {
        id: 'address',
        name: 'כתובת החוזה',
        showColumn: false,
      },
    ];
  }
  }
  getContractsBySupplier() {
    const url = environment.serverUrl + 'contracts/getContractsBySupplier';
    return this.httpClient.post<Contract[]>(url, this.supplier);
  }
  addContract(contract: Contract) {
    const url = environment.serverUrl + 'contracts/addContract';
    return this.httpClient.post<boolean>(url, contract);
  }
  deleteContract(contract: Contract) {
    const url = environment.serverUrl + 'contracts/removeContract';
    return this.httpClient.post<boolean>(url, contract);
  }
  editContract(contract: Contract) {
    const url = environment.serverUrl + 'contracts/editContract';
    return this.httpClient.post<boolean>(url, contract);
  }

}
