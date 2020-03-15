import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ObjectHandlerService {

  constructor() { }
  capitlizeObjectProperties(obj: object) {
    const keys = Object.keys(obj);
    let key = '';
    let n = keys.length;
    const newobj = {};
    while (n--) {
      key = keys[n];
      newobj[key.replace(/^\w/, c => c.toUpperCase())] = obj[key];
    }
    return newobj;
  }
}
