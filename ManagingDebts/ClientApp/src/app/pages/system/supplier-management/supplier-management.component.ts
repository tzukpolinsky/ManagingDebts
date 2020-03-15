import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import {  SwalPortalTargets } from '@sweetalert2/ngx-sweetalert2';
import { Supplier } from '../../../entites/supplier';
import { Subject } from 'rxjs';
import { Msg } from '../../../entites/msg';
import { TableHeader } from '../../../entites/table-headers';
import { TableBtn } from '../../../entites/table-btn';
import { SupplierService } from '../../../services/supplier.service';
import { takeUntil } from 'rxjs/operators';
import { isNumber } from 'util';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-supplier-managment',
  templateUrl: './supplier-management.component.html',
})
export class SupplierManagementComponent implements OnInit, OnDestroy {
  suppliers: Supplier[];
  destroyed = new Subject();
  newSupplier: Supplier;
  msg: Msg;
  isLoading: boolean;
  editSupplier: Supplier;
  tableHeaders: TableHeader[];
  editBtn: TableBtn;
  public constructor( public readonly swalTargets: SwalPortalTargets,
                      private supplierService: SupplierService) { }
  ngOnDestroy(): void {
    this.destroyed.next();
  }
  ngOnInit() {
    this.getSuppliers();
    this.newSupplier = this.supplierService.createEmptyEntity();
    this.editSupplier = this.supplierService.createEmptyEntity();
    this.tableHeaders = this.supplierService.createTableHeaders();
    this.editBtn = {
      text: 'ערוך',
      type: 'warning',
    };
  }
  private getSuppliers() {
    this.supplierService.getSupplierByCustomer()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.suppliers = result;
    });
  }
  add() {
    this.isLoading = true;
    if (!isNaN(Number(this.newSupplier.id)) && this.supplierService.checkId(parseInt(this.newSupplier.id , 10))) {
      if (!this.suppliers.find(x => parseInt(x.id, 10) === parseInt(this.newSupplier.id, 10))) {
      this.newSupplier.pkudatYomanNumber = parseInt(this.newSupplier.pkudatYomanNumber.toString(), 10);
      this.supplierService.addSupplier(this.newSupplier)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          type: result ? 'success' : 'danger',
          text: result ? 'הספק נוסף בהצלחה' : 'קרתה תקלה בעת הוספת הספק',
          dismissible: true,
        };
        this.newSupplier = this.supplierService.createEmptyEntity();
        this.isLoading = false;
        if (result) {
          this.refreshWindow();
        }
      });
    } else {
      this.msg = {
        type: 'danger',
        text: 'קיים ספק עם מזהה זה',
        dismissible: true,
      };
      this.isLoading = false;
    }
    } else {
      this.msg = {
        type: 'danger',
        text: 'ת.ז אינה תקינה',
        dismissible: true,
      };
      this.isLoading = false;
    }

  }
  refreshWindow() {
    let timerInterval;
    Swal.fire({
      title: 'המערכת צריכה לבצע רענון',
      html: 'הרענון יתבצע בעוד <b></b> מילי-שניות',
      timer: 3000,
      timerProgressBar: true,
      onBeforeOpen: () => {
        Swal.showLoading();
        timerInterval = setInterval(() => {
          const content = Swal.getContent();
          if (content) {
            const b = content.querySelector('b');
            if (b) {
              b.textContent = Swal.getTimerLeft().toString();
            }
          }
        }, 100);
      },
      onClose: () => {
        clearInterval(timerInterval);
      }
    }).then((result) => {
      window.location.reload();
    });
  }
  popUpEditWindow(supplier: any) {
    this.editSupplier = {
      id: supplier.id,
      name: supplier.name,
      isEnable: supplier.isEnable,
      supplierNumberInFinance: supplier.supplierNumberInFinance,
      withBanks: supplier.withBanks,
      pkudatYomanNumber: supplier.pkudatYomanNumber,
    };
  }
  clearEditSupplier() {
    this.editSupplier = this.supplierService.createEmptyEntity();
  }
  edit() {
    this.isLoading = true;
    this.editSupplier.pkudatYomanNumber = parseInt(this.editSupplier.pkudatYomanNumber.toString(), 10);
    this.supplierService.editSupplier(this.editSupplier)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.msg = {
        type: result ? 'success' : 'danger',
        text: result ? 'הספק עודכן בהצלחה' : 'קרתה תקלה בעת עדכון הספק',
        dismissible: true,
      };
      this.editSupplier = this.supplierService.createEmptyEntity();
      this.isLoading = false;
      if (result) {
        this.refreshWindow();
      }
    });
  }

}
