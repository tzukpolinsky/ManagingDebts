import { Component, OnInit, OnDestroy } from '@angular/core';
import { BankAccountService } from '../../../services/bank-account.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BankAccount } from '../../../entites/bank-account';
import { Msg } from '../../../entites/msg';
import { TableBtn } from '../../../entites/table-btn';
import { TableHeader } from '../../../entites/table-headers';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-bank-managment',
  templateUrl: './bank-management.component.html',
})
export class BankManagementComponent implements OnInit , OnDestroy {

  destroyed = new Subject();
  banks: BankAccount[];
  newBank: any;
  banksList: any;
  branchList: any;
  selectedNewBank: number;
  deleteBtn: TableBtn;
  tableHeaders: TableHeader[];
  fileData: any;
  msg: Msg;
  isLoading: boolean;
  constructor(private bankService: BankAccountService) { }
  ngOnDestroy(): void {
    this.destroyed.next();
  }
  ngOnInit() {
    this.fileData = this.bankService.getBankAndBranchesName();
    this.getDataBySupplier();
    this.banksList = [];
    this.fileData.BRANCHES.BRANCH.forEach(ele => {
      if (!this.banksList.find(x => x.Bank_Code.trim() === ele.Bank_Code.trim())) {
        this.banksList.push(ele);
      }
    });
    this.newBank = this.bankService.createEmptyEntity();
    this.deleteBtn = {
      text: 'מחק בנק',
      type: 'danger',
    };
  }
  private getDataBySupplier() {
    this.bankService.getBySupplier()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      if (result) {
      this.banks = result;
      this.tableHeaders = this.bankService.getBankTableHeaders();
    }
    });
  }
  addBank() {
    this.isLoading = true;
    this.bankService.addBank(this.newBank)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.getDataBySupplier();
      this.msg = {
        text: result ? 'הבנק נוסף בהצלחה' : 'הוספת הבנק נכשלה',
        type: result ? 'success' : 'danger',
        dismissible: true,
      };
      this.newBank = {};
      this.isLoading = false;
    });
  }
  deleteBank(bank: BankAccount) {
    this.isLoading = true;
    Swal.fire({
      title: 'בטוחים?',
      text: 'כל החוזים הקשורים לבנק זה יהיו ללא בנק',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'כן, מחק',
      cancelButtonText: 'לא, בטל את הפעולה'
    }).then((reply) => {
      if (reply.value) {
        this.bankService.deleteBank(bank)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.getDataBySupplier();
      this.msg = {
        text: result ? 'הבנק נמחק בהצלחה' : 'מחיקת הבנק נכשלה',
        type: result ? 'success' : 'danger',
        dismissible: true,
      };
      this.isLoading = false;
    });
      } else {
        this.isLoading = false;
      }});
  }
}
