import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../entites/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SendMailService {

  constructor(private httpClient: HttpClient) { }
  reportManualChangeInLocaleStorage(user: User){
    const url = environment.serverUrl + 'mail/manualLocalStorageChange';
    return this.httpClient.post<boolean>(url, user);
  }
  reportError(user: User) {
    const url = environment.serverUrl + 'mail/reportError';
    return this.httpClient.post<boolean>(url, user);
  }
  reportAccessDenied(user: User) {
    const url = environment.serverUrl + 'mail/reportAccessDenied';
    return this.httpClient.post<boolean>(url, user);
  }
}
