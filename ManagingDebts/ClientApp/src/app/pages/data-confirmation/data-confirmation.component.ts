import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataConfirmationService } from '../../services/data-confirmation.service';
import { BillingSummary } from '../../entites/billing-summary';
import { takeUntil} from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Msg } from '../../entites/msg';
import { ObjectHandlerService } from '../../services/object-handler.service';
import { TableHeader } from '../../entites/table-headers';
import { NgxSpinnerService } from 'ngx-spinner';
import { BillingSummaryService } from '../../services/billing-summary.service';
import { Contract } from '../../entites/contract';
import { ContractService } from '../../services/contract.service';
import { SwalComponent } from '@sweetalert2/ngx-sweetalert2';
@Component({
  selector: 'app-data-confirmation',
  templateUrl: './data-confirmation.component.html',
})
export class DataConfirmationComponent implements OnInit, OnDestroy {
  p = 1;
  displayNotMatchOnly: boolean;
  data: any;
  contracts: Contract[];
  newContract: Contract;
  displayData: any;
  searchValues: any;
  msg: Msg;
  searchMsg: Msg;
  summaries: BillingSummary[];
  selectedSummaryRowId: number;
  tableHeaders: TableHeader[];
  selected: TableHeader[];
  selectedSummary: BillingSummary;
  sumCredit: number;
  sumDebit: number;
  isLoading: boolean;
  @ViewChild('contractAddtion', {static: true }) private contractAddtion: SwalComponent;
  private destroyed = new Subject();
  constructor(private dataConfirmationService: DataConfirmationService, private contractService: ContractService,
              private objectHandlerServive: ObjectHandlerService, private spinner: NgxSpinnerService,
              private billingSummaryService: BillingSummaryService) { }
  ngOnDestroy(): void {
    this.destroyed.next();
  }
  ngOnInit() {
    this.getAllContracts();
    this.displayNotMatchOnly = false;
    this.selectedSummary = this.billingSummaryService.createEmptyEntity();
    this.billingSummaryService.GetAllSummaryByCustomer()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.summaries = result.filter(x => !x.isSent);
      this.summaries.sort((x, y) => x.dateOfValue > y.dateOfValue ? 1 : -1);
      this.selectedSummary = this.summaries.find(x => !x.isSent);
      this.dataConfirmationService.getDataBySummary(this.selectedSummary)
      .pipe(takeUntil(this.destroyed))
      .subscribe(data => this.handleData(data));
      if (this.summaries && this.summaries.length > 0) {
        this.selectedSummaryRowId = this.summaries[0].rowId;
      }
     } );
    this.tableHeaders = this.dataConfirmationService.getTableHeaders();
    this.selected = [];
    this.tableHeaders.forEach(head => {
      if (head.showColumn) {
        this.selected.push(head);
      }
      head.isAsc = true;
    });
    }
    getAllContracts() {
      this.contractService.getContractsBySupplier()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => this.contracts = result);
    }
    sortBy(id: string, isAsc: boolean) {
      isAsc ? this.data.sort((x, y) => x[id] <= y[id] ? 1 : -1) : this.data.sort((x, y) => x[id] >= y[id] ? 1 : -1);
      this.tableHeaders.find(x => x.id === id).isAsc = !isAsc;
    }
    private handleData(result: any) {
      if (result) {
        if (!this.contracts) {
        this.contractService.getContractsBySupplier()
        .pipe(takeUntil(this.destroyed))
        .subscribe(resultContract => {
        this.contracts = resultContract;
        this.createRowsFromData(result);
    });
      } else {
        this.createRowsFromData(result);
      }
        this.sumCredit = this.selectedSummary.totalDebit;
    }
  }
    private isNewContract(summary: any) {
        const result = this.contracts.find(x => x.id === summary.contract);
        summary.isNewContract = !!!result;
    }
    private createRowsFromData(result: any) {
      const summaryRows = Object.keys(result);
      this.data = [];
      this.displayData = [];
      this.sumDebit = 0;
      summaryRows.forEach(ele => {
        this.data.push({
          summary: JSON.parse(ele),
          data: result[ele],
          showData: false,
        });
        this.tableHeaders.forEach((row) => {
          // making sure there is no 'empty' values in dispaly
          this.data[this.data.length - 1].summary[row.id] =
          this.data[this.data.length - 1].summary[row.id] ?
          this.data[this.data.length - 1].summary[row.id] : '';
        });
        this.data[this.data.length - 1].data.forEach((row) => {
          this.sumDebit += row.isMatched ? row.amountAfterTax : 0;
        });
        this.isNewContract(this.data[this.data.length - 1].summary);
      });
      if (this.data && this.data.length > 0) {
        this.searchValues = JSON.parse(JSON.stringify(this.data[0].summary));
      } else {
        this.searchValues = null;
      }
      this.clearSearch();
    }
    popUpContractAdd(row: any) {
      this.newContract = this.contractService.createEmptyEntity();
      this.newContract.id = row.summary.contract;
      this.contractAddtion.fire();
    }
    clearSearch() {
      for (const attr in this.searchValues) {
        if (true) {// for tslint
          this.searchValues[attr] = null;
        }
      }
      this.displayData = this.data;
    }
    match() {
      this.isLoading = true;
      this.spinner.show();
      this.dataConfirmationService.match(this.data.filter(x => !x.summary.isNewContract))
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          type: result ? 'success' : 'danger',
          text: result ? 'ההתאמה בוצעה בהצלחה' : 'קרתה תקלה בעת ביצוע ההתאמה',
          dismissible: true,
        };
        this.dataConfirmationService.getDataBySummary(this.selectedSummary)
        .pipe(takeUntil(this.destroyed))
        .subscribe(newData => {
          this.handleData(newData);
          this.isLoading = false;
          this.spinner.hide();
        });

      });
    }
    toggleRowData(row: any, $event) {
      $event.toElement.className =  !row.showData ? 'tim-icons icon-minimal-up' : 'tim-icons icon-minimal-down';
      row.showData = !row.showData;
    }
    checkContract(row: any) {
      const state: boolean = !row.summary.isMatched;
      row.summary.isMatched = state;
      row.data.forEach(ele => {
        const amount = ele.amountAfterTax;
        this.sumDebit += state ? amount : -1 * amount;
        ele.isMatched = state;
      });
    }
    checkAll($event) {
      const state: boolean = $event.currentTarget.checked;
      this.sumDebit = 0;
      this.data.forEach(ele => {
        if (!ele.summary.isNewContract) {
          ele.data.forEach(row => {
            this.sumDebit += state ? row.amountAfterTax : 0;
            row.isMatched = state;
          });
          ele.summary.isMatched = state;
        }
      });
    }
    searchBySummary() {
      this.isLoading = true;
      this.spinner.show();
      this.dataConfirmationService.getDataBySummary( this.selectedSummary)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.handleData(result);
        this.isLoading = false;
        this.spinner.hide();
  });
    }
    search() {
      let filterData = this.data;
      this.tableHeaders.forEach(search => {
        if (this.searchValues[search.id]) {
          filterData = filterData.filter(x => x.summary[search.id].toString().includes(this.searchValues[search.id]));
        }
      });
      if (filterData && filterData.length > 0) {
        this.displayData = filterData;
      } else {
        this.clearSearch();
        this.searchMsg = {
          text: 'לא קיימים נתונים לפי החיפוש',
          type: 'warning',
          dismissible: true
        };
      }

    }
    removeNewContractAttr(result: boolean) {
      if (result) {
        this.data.find(x => x.summary.contract === this.newContract.id).summary.isNewContract = false;
      }
    }
    getTotal() {
      const total = this.sumCredit - this.sumDebit;
      if (Math.abs(total) < 0.01 ) {
        return 0;
      }
      return total;
    }
    assignToSummary() {
      this.selectedSummary = this.summaries.find(x => x.rowId === parseInt(this.selectedSummaryRowId.toString(), 10));
    }
  }
