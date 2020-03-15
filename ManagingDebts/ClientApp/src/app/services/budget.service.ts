import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Budget } from '../entites/budget';
import { TableHeader } from '../entites/table-headers';
import { environment } from '../../environments/environment';
import { Contract } from '../entites/contract';
import { SupplierService } from './supplier.service';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {

  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  supplier = this.supplierService.getCurrentSupplier();
  createEmptyEntity(): Budget {
    return {
      id: 0,
      customerId: this.supplier.customerId,
      supplierId: this.supplier.id,
      name: '',
    };

  }
  getBudgetTableHeaders(): TableHeader[] {
    return [
      {
        id: 'id',
        name: 'מזהה סעיף',
        showColumn: true,
      },
      {
        id: 'name',
        name: 'תיאור הסעיף',
        showColumn: true,
      },
      {
        id: 'contracts',
        name: 'חוזים',
        showColumn: true,
      },
    ];
  }
  getBySupplier() {
    const url = environment.serverUrl + 'budgets/getBySupplier';
    return this.httpClient.post<Budget[]>(url,  this.supplier);
  }
  getBudgetsFromFinance() {
    const url = environment.serverUrl + 'budgets/getByContract';
    return this.httpClient.post<Budget[]>(url, this.createEmptyEntity());
  }
  convertFileToBudgetsEntities(file: File) {
    const formData = new FormData();
    formData.append('file', file, file.name);
    const headers = new HttpHeaders().append('Content-Disposition', 'multipart/form-data');
    const url = environment.serverUrl + 'budgets/convertFileToBudgetsEntities';
    return this.httpClient.post<Budget[]>(url, formData, {headers});
  }
  addBudgets(budgets: Budget[]) {
    const url = environment.serverUrl + 'budgets/addBudgets';
    return this.httpClient.post<boolean>(url, budgets);
  }
  addBudget(budget: Budget) {
    const url = environment.serverUrl + 'budgets/addBudget';
    return this.httpClient.post<boolean>(url, budget);
  }
  deleteBudget(budget: Budget) {
    const url = environment.serverUrl + 'budgets/removeBudget';
    return this.httpClient.post<boolean>(url, budget);
  }
  editBudget(budget: Budget) {
    const url = environment.serverUrl + 'budgets/editBudget';
    return this.httpClient.post<boolean>(url, budget);
  }
  uploadRelations(uploadFile: File) {
    const formData = new FormData();
    formData.append('file', uploadFile, uploadFile.name);
    const headers = new HttpHeaders().append('Content-Disposition', 'multipart/form-data');
    const url = environment.serverUrl + 'contracts/uploadRelations/'
    +  this.supplier.customerId + '/' +  this.supplier.id;
    return this.httpClient.post<boolean>(url, formData, {headers});
  }
}
