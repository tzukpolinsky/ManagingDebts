import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Customer } from '../entites/customer';
import { User } from '../entites/user';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private httpClient: HttpClient) { }
  createEmptyEntity(): Customer {
    return {
      id: 0,
      name: '',
      isActive: true,
    };
  }
  getByUser() {
    const url = environment.serverUrl + 'customer/getByUser';
    const user = JSON.parse(localStorage.getItem('user'));
    return this.httpClient.post<Customer[]>(url, user);
  }
  getAll() {
    const url = environment.serverUrl + 'customer/getAllCustomers';
    return this.httpClient.get<Customer[]>(url);
  }
}
