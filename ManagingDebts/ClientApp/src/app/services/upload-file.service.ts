import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Supplier } from '../entites/supplier';
import { SupplierService } from './supplier.service';
@Injectable({
  providedIn: 'root'
})
export class UploadFileService {
  supplier = this.supplierService.getCurrentSupplier();

  constructor(private httpClient: HttpClient, private supplierService: SupplierService) { }
  checkFileTypeBySupplier(file: File): boolean {
    switch (this.supplier.id) {
      case '520031931':
        return file.type.includes('spreadsheetml') || file.name.endsWith('.xlsx');
      case '520000472':
        return file.type.includes('ms-excel') || file.name.endsWith('.csv');
      default:
        return file.type.includes('spreadsheetml') || file.name.endsWith('.xlsx');
    }
  }
  uploadFile(uploadFile: File) {
    const formData = new FormData();
    formData.append('file', uploadFile, uploadFile.name);
    const headers = new HttpHeaders().append('Content-Disposition', 'multipart/form-data');
    const url = environment.serverUrl + 'uploadFile/uploadFile/'
    + this.supplier.customerId + '/' + this.supplier.id;
    return this.httpClient.post<boolean>(url, formData, {headers});
  }
}
