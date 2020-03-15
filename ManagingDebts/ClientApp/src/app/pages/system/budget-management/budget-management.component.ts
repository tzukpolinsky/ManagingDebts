import {
  Component,
  OnInit,
  OnDestroy
} from '@angular/core';
import {
  Budget
} from '../../../entites/budget';
import {
  Contract
} from '../../../entites/contract';
import {
  BudgetService
} from '../../../services/budget.service';
import {
  Subject
} from 'rxjs';
import {
  takeUntil,
} from 'rxjs/operators';
import {
  TableHeader
} from '../../../entites/table-headers';
import {
  TableBtn
} from '../../../entites/table-btn';
import {
  ContractService
} from '../../../services/contract.service';
import {
  Msg
} from '../../../entites/msg';
import {
  SwalPortalTargets
} from '@sweetalert2/ngx-sweetalert2';
import {
  NgxSpinnerService
} from 'ngx-spinner';
import Swal from 'sweetalert2';
import { SupplierService } from '../../../services/supplier.service';

@Component({
  selector: 'app-budget-managment',
  templateUrl: './budget-management.component.html',
})
export class BudgetManagementComponent implements OnInit, OnDestroy {
  budgets: Budget[];
  budgetsToDisplay: any[];
  contracts: Contract[];
  newBudget: Budget;
  selectedBudget: Budget;
  budgetsToSelect: Budget[];
  UnusualBudgetsToSelect: Budget[];
  selectedBudgets: Budget[];
  UnusualSelectedBudgets: Budget[];
  selectedContract: Contract[];
  selectedFile: File;
  selectedRelationFile: File;
  isLoading: boolean;
  isLoadingStateChange: boolean;
  msg: Msg;
  fileUploadMsg: Msg;
  tableHeaders: TableHeader[];
  deleteBtn: TableBtn;
  editBtn: TableBtn;
  destroyed = new Subject();
  constructor(public readonly swalTargets: SwalPortalTargets, private spinner: NgxSpinnerService,
              private budgetService: BudgetService, private contractService: ContractService, private supplierService: SupplierService) {}

  ngOnInit() {
    this.getBySupplier();
    this.getContractsBySupplier();
    this.newBudget = this.budgetService.createEmptyEntity();
    this.tableHeaders = this.budgetService.getBudgetTableHeaders();
    this.editBtn = {
      text: 'ערוך',
      type: 'warning'
    };
    this.deleteBtn = {
      text: 'מחק',
      type: 'danger'
    };
  }
  ngOnDestroy(): void {
    this.destroyed.next();
  }
  clearBudget() {
    this.selectedBudget = null;
  }
  getContractsBySupplier() {
    this.contractService.getContractsBySupplier()
    .pipe(takeUntil(this.destroyed))
    .subscribe(result => {
      this.contracts = result;
      this.contracts.forEach((contract) => {
         contract.description += ' - ' + contract.id;
      });
    });
  }
  getBySupplier() {
    this.spinner.show();
    this.isLoading = true;
    this.budgetService.getBySupplier()
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.budgets = result;
        const interval = setInterval(() => {
          if (this.contracts) {
            this.createDisplayData();
            clearInterval(interval);
          }
        }, 10);
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  private createDisplayData() {
    this.budgetsToDisplay = [];
    this.budgets.forEach((budget) => {
      const budgetToDisplay = {
        contracts: '',
        name: budget.name,
        id: budget.id
      };
      this.contracts.forEach((contract) => {
        contract.budgetContract.forEach(budgetContract => {
          if (budgetContract.budgetId === budget.id) {
            budgetToDisplay.contracts += budgetContract.contractId + ',';
          }
        });
      });
      budgetToDisplay.contracts = budgetToDisplay.contracts.substring(0, budgetToDisplay.contracts.length - 1 );
      budgetToDisplay.contracts = budgetToDisplay.contracts ? budgetToDisplay.contracts : 'לא מקושר לחוזה';
      this.budgetsToDisplay.push(budgetToDisplay);
    });
  }
  clearContractSearch() {
    this.isLoading = true;
    this.selectedContract = null;
    this.getBySupplier();
  }
  handleBudgetsToSelect(budgets: Budget[]) {
    this.budgetsToSelect = [];
    this.UnusualBudgetsToSelect = [];
    const supplier = this.supplierService.getCurrentSupplier();
    const customerId = parseInt(localStorage.getItem('customerId'), 10);
    budgets.forEach((budget) => {
      budget.name += '-' + budget.id;
      budget.supplierId = supplier.id;
      budget.customerId = customerId;
      if (!this.budgets.find(x => x.id === budget.id)) {
        if (budget.id >= 1000000000 && budget.id < 2000000000) {
          this.budgetsToSelect.push(budget);
        }
      } else if (budget.id >= 2000000000 && budget.id < 3000000000) {
        this.UnusualBudgetsToSelect.push(budget);
      }
    });
    this.fileUploadMsg = {
      text: budgets.length > 0 ? 'ניתן לבחור סעיפים' : 'אין סעיפים זמינים',
      type: budgets.length > 0 ? 'success' : 'danger',
      dismissible: true
    };
  }
  getBudgetsFromFinance() {
    this.isLoading = true;
    this.budgetService.getBudgetsFromFinance()
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.handleBudgetsToSelect(result);
        this.isLoading = false;
      });
  }
  getRelationShipsFromFile() {
    this.isLoading = true;
    this.spinner.show();
    this.budgetService.uploadRelations(this.selectedRelationFile)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'יחסי חוזים - סעיפים תקציביים הועלו בהצלחה' : 'קרתה תקלה בעת העלת הקובץ',
          type: result ? 'success' : 'danger',
          dismissible: true
        };
        if (result) {
          this.getBySupplier();
          this.getContractsBySupplier();
          this.selectedRelationFile = null;
        }
      });
  }
  onRelationsFileChange(event) {
    this.selectedRelationFile = event.target.files[0];
    if (!this.selectedRelationFile.type.includes('spreadsheetml') && !this.selectedRelationFile.name.endsWith('.xlsx')) {
      Swal.fire({
        title: 'לא ניתן להעלות את הקובץ',
        text: 'לא ניתן להעלות קובץ מסוג ' + this.selectedRelationFile.type,
        icon: 'warning',
      });
      this.selectedRelationFile = null;
    }
  }
  onFileChange(event) {
    this.selectedFile = event.target.files[0];
    if (!this.selectedFile.type.includes('spreadsheetml') && !this.selectedFile.name.endsWith('.xlsx')) {
      Swal.fire({
        title: 'לא ניתן להעלות את הקובץ',
        text: 'לא ניתן להעלות קובץ מסוג ' + this.selectedFile.type,
        icon: 'warning',
      });
      this.selectedFile = null;
    }
  }
  getBudgetsFromFile() {
    this.isLoading = true;
    this.spinner.show();
    this.budgetService.convertFileToBudgetsEntities(this.selectedFile)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.handleBudgetsToSelect(result);
        this.selectedFile = null;
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  addBudgets() {
    this.isLoadingStateChange = true;
    this.spinner.show();
    this.budgetService.addBudgets(this.UnusualSelectedBudgets &&
        this.UnusualSelectedBudgets.length > 0 ?
        this.selectedBudgets.concat(this.UnusualSelectedBudgets) : this.selectedBudgets)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'הוספת הסעיפים בוצעה בהצלחה' : 'קרתה תקלה בעת הוספת הסעיפים',
          type: result ? 'success' : 'danger',
          dismissible: true
        };
        if (result) {
          this.getBySupplier();
        }
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  addBudget() {
    if (!this.budgets.find(x => x.id === parseInt(this.newBudget.id.toString(), 10))) {
    this.isLoadingStateChange = true;
    this.spinner.show();
    this.budgetService.addBudget(this.newBudget)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'הוספת הסניף בוצעה בהצלחה' : 'קרתה תקלה בעת הוספת הסניף',
          type: result ? 'success' : 'danger',
          dismissible: true
        };
        if (result) {
          this.getBySupplier();
        }
        this.isLoading = false;
        this.spinner.hide();
      });
    } else {
      this.msg = {
        text: 'התקציב כבר קיים',
        type: 'danger',
        dismissible: false
      };
    }
  }
  popUpDeleteBudget(budget: Budget) {
    Swal.fire({
      title: 'בטוחים?',
      text: 'כל הקשרי חוזים עם הסעיף הזה יתנתקו',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'כן, מחק',
      cancelButtonText: 'לא, בטל את הפעולה'
    }).then((reply) => {
      if (reply.value) {
        this.deleteBudget(budget);
      }
    });
  }
  deleteBudget(budget: Budget) {
    this.isLoadingStateChange = true;
    this.spinner.show();
    this.budgetService.addBudget(budget)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'מחיקת הסניף בוצעה בהצלחה' : 'קרתה תקלה בעת מחיקת הסניף',
          type: result ? 'success' : 'danger',
          dismissible: true
        };
        if (result) {
          this.getBySupplier();
        }
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  popUpEditWindow(budget: Budget) {
    this.selectedBudget = budget;
  }
  editBudget(budget: Budget) {
    this.isLoadingStateChange = true;
    this.spinner.show();
    this.budgetService.addBudget(budget)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'עריכת הסניף בוצעה בהצלחה' : 'קרתה תקלה בעת עריכת הסניף',
          type: result ? 'success' : 'danger',
          dismissible: true
        };
        if (result) {
          this.getBySupplier();
        }
        this.spinner.hide();
        this.isLoading = false;
      });
  }
}
