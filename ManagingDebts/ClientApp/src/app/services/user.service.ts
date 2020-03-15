import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../entites/user';
import { environment } from '../../environments/environment';
import { CustomerService } from './customer.service';
import { TableHeader } from '../entites/table-headers';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient, private customerService: CustomerService) { }
  createEmptyEntity(): User {
    const customerInSession = localStorage.getItem('customerId');
    return {
      id: '',
      firstName: '',
      lastName: '',
      customerId: customerInSession ? parseInt(customerInSession, 10) : 0,
      isActive: true,
      email: '',
      isSuperAdmin: false,
    };
  }
  createTableHeaders(): TableHeader[] {
    return [
      {
        id: 'id',
        name: 'מזהה משתמש',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'firstName',
        name: 'שם פרטי',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'lastName',
        name: 'שם משפחה',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'email',
        name: 'אימייל המשתמש',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'customerId',
        name: 'רשות המשתמש',
        showColumn: true,
        isAsc: true,
      },
      {
        id: 'isActive',
        name: 'האם פעיל',
        showColumn: true,
        isAsc: true,
      },
    ];
  }
  getAll() {
    const url = environment.serverUrl + 'users/getAll';
    return this.httpClient.get<User[]>(url);
  }
  changePassword(users: User[]) {
    users[0].password = btoa(users[0].password);
    users[1].password = btoa(users[1].password);
    const url = environment.serverUrl + 'users/changePassword';
    return this.httpClient.post<any>(url, users);
  }
  createUser(user: User, password: string) {
    user.password = btoa(password);
    const url = environment.serverUrl + 'users/createUser';
    return this.httpClient.post<boolean>(url, user);
  }
  login(user: User) {
    user.password = btoa(user.password);
    const url = environment.serverUrl + 'users/login';
    return this.httpClient.post<any>(url , user);
  }
  logOut() {
    localStorage.setItem('user', '');
    window.location.assign('/');
  }
  editUser(user: User) {
    const url = environment.serverUrl + 'users/editUser';
    return this.httpClient.post<boolean>(url , user);
  }
}
