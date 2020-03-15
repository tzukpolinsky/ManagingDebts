import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnChanges
} from '@angular/core';
import {
  IDropdownSettings
} from 'ng-multiselect-dropdown';
import {
  TableHeader
} from '../../../entites/table-headers';
import {
  TableBtn
} from '../../../entites/table-btn';
import {
  NgxSpinnerService
} from 'ngx-spinner';
import {
  Msg
} from '../../../entites/msg';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
})
export class TableComponent implements OnInit, OnChanges {
  p = 1;
  @Input() isLoading: boolean;
  @Input() data: any[];
  @Input() tableBtnFirst: TableBtn;
  @Input() tableBtnSecond: TableBtn;
  @Input() dataHeaders: TableHeader[];
  @Output() firstBtnClicked = new EventEmitter < any > ();
  @Output() secondBtnClicked = new EventEmitter < any > ();
  displayData: any;
  searchValues: any;
  keys = Object.keys;
  msg: Msg;
  dropdownSettings: IDropdownSettings = {};
  selected: TableHeader[];
  constructor(private spinner: NgxSpinnerService) {}
  ngOnChanges() {
    this.isLoading ? this.spinner.show() : this.spinner.hide();
    if (this.data && this.data.length > 0 && this.dataHeaders && this.dataHeaders.length > 0) {
      this.data.forEach((ele) => {
        this.dataHeaders.forEach((head) => {
          ele[head.id] = ele[head.id] !== null && ele[head.id] !== undefined ? ele[head.id] : '';
        });
      });
      this.displayData = this.data;
      this.setSelected();
      this.sortBy(this.dataHeaders[0].id, this.dataHeaders[0].isAsc);
      this.searchValues = JSON.parse(JSON.stringify(this.data[0]));
      this.clearSearch();
    }
  }
  ngOnInit() {
    if (this.data && this.data.length > 0 && this.dataHeaders && this.dataHeaders.length > 0) {
      this.setSelected();
      this.sortBy(this.dataHeaders[0].id, this.dataHeaders[0].isAsc);
    }
    this.isLoading = this.isLoading;
  }
  sortBy(id: string, isAsc: boolean) {
    isAsc ? this.data.sort((x, y) => x[id] <= y[id] ? 1 : -1) : this.data.sort((x, y) => x[id] >= y[id] ? 1 : -1);
    this.dataHeaders.find(x => x.id === id).isAsc = !isAsc;
  }
  firstClicked(row: any) {
    this.firstBtnClicked.emit(row);
  }
  secondClicked(row: any) {
    this.secondBtnClicked.emit(row);
  }

  private setSelected() {
    if (this.dataHeaders && this.dataHeaders.length > 0) {


      this.selected = [];
      this.dataHeaders.forEach(head => {
        if (head.showColumn) {
          this.selected.push(head);
        }
        head.isAsc = true;
      });
    }
  }
  clearSearch() {
    for (const attr in this.searchValues) {
      if (true) { // for tslint
        this.searchValues[attr] = null;
      }
    }
    this.displayData = this.data;
  }
  search() {
    let filterData = this.data;
    this.dataHeaders.forEach(search => {
      if (this.searchValues[search.id]) {
        filterData = filterData.filter(x => x[search.id].toString().includes(this.searchValues[search.id]));
      }
    });
    if (filterData && filterData.length > 0) {
      this.displayData = filterData;
    } else {
      this.clearSearch();
      this.msg = {
        text: 'לא קיימים נתונים לפי החיפוש',
        type: 'warning',
        dismissible: true
      };
    }
  }
  booleanCondation(data: any): boolean {
    return typeof data === 'boolean';
  }
  dateCondation(data: any): boolean {
    return data.includes !== undefined && data.includes('T');
  }
}
