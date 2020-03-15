import { Component, OnInit, OnDestroy } from '@angular/core';
import { BillingSummary } from '../../entites/billing-summary';
import { Msg } from '../../entites/msg';
import { NgxSpinnerService } from 'ngx-spinner';
import { TableHeader } from '../../entites/table-headers';
import { BillingSummaryService } from '../../services/billing-summary.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { SliService } from '../../services/sli.service';
import { User } from '../../entites/user';
import { SupplierService } from '../../services/supplier.service';
import { Supplier } from '../../entites/supplier';
import { UserService } from '../../services/user.service';
import { SwalPortalTargets } from '@sweetalert2/ngx-sweetalert2';

@Component({
  selector: 'app-sli',
  templateUrl: './sli.component.html'
})
export class SliComponent implements OnInit, OnDestroy {
  destroyed = new Subject();
  now = new Date();
  selectedYear: number;
  selectedMonth: number;
  years: number[];
  months: number[];
  data: any;
  dataHeader: TableHeader[];
  sumDebit: number;
  sumCredit: number;
  summaries: BillingSummary[];
  selectedSummary: BillingSummary;
  selectedSummaryRowId: number;
  financeUser: User;
  supplier: Supplier = this.supplierService.getCurrentSupplier();
  msg: Msg;
  isLoading: boolean;

  constructor(private spinner: NgxSpinnerService,
              private summariesService: BillingSummaryService,
              private sliService: SliService,
              private supplierService: SupplierService,
              private userService: UserService,
              public readonly swalTargets: SwalPortalTargets) { }

  ngOnInit() {
    this.summariesService.GetAllSummaryByCustomer()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      if (result && result.length > 0) {
        this.summaries = result.filter(x => !x.isSent);
        this.selectedSummary = this.summaries[0];
        this.selectedSummaryRowId = this.summaries[0].rowId;
      }
    });
    this.dataHeader = this.sliService.createTableHeaders();
    const range = (start, len, step) => Array.from({ length: len}, (_, i) => start + (i * step));
    this.years = range(this.now.getFullYear(), 7, -1);
    this.months = range(1, 12, 1);
    this.selectedYear = new Date(this.now.setMonth(this.now.getMonth() - 1)).getUTCFullYear();
    this.now.setMonth(this.now.getMonth() + 1);
    this.selectedMonth = new Date(this.now.setMonth(this.now.getMonth() - 1)).getMonth() + 1;
    this.now.setMonth(this.now.getMonth() + 1);
  }
  ngOnDestroy() {
    this.destroyed.next();
  }

  displaySli() {
    this.isLoading = true;
    this.spinner.show();
    const dateToSent = new Date(this.selectedYear, this.selectedMonth, 0);
    this.sliService.displaySli(this.selectedSummary, dateToSent)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.data = result;
      this.sumCredit = 0;
      this.sumDebit = 0;
      this.data.forEach((element) => {
        if (element.zchutChovaInd === 2) {
          this.sumCredit += element.schum;
        } else if (element.zchutChovaInd === 1) {
          this.sumDebit += element.schum;
        }
      });
      this.isLoading = false;
      this.spinner.hide();
    });
  }
  getFinanceUser() {
    this.financeUser = this.userService.createEmptyEntity();
  }
  createSli() {
    this.isLoading = true;
    this.spinner.show();
    const dateToSent = new Date(this.selectedYear, this.selectedMonth, 0);
    this.sliService.createSli(this.selectedSummary, dateToSent, this.financeUser)
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.msg = {
        type: result.isSuccess ? 'success' : 'danger',
        text: result.msg,
        dismissible : false
      };
      this.financeUser = null;
      this.isLoading = false;
      this.spinner.hide();
    });
  }
  selectSummary() {
    this.selectedSummary = this.summaries.find(x => x.rowId === parseInt(this.selectedSummaryRowId.toString(), 10));
  }
  getTotal() {
    const total = this.sumCredit - this.sumDebit;
    if (Math.abs(total) < 0.01 ) {
      this.sumDebit = this.sumCredit; // prevent dispaly differnces in 0.01 postion
      return 0;
    }
    return total;
  }
}
