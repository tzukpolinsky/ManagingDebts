import {
  Component,
  OnInit,
  Input,
  OnDestroy,
  Output,
  EventEmitter,
  ViewChild
} from '@angular/core';
import {
  ContractService
} from '../../../services/contract.service';
import {
  BudgetService
} from '../../../services/budget.service';
import {
  Contract
} from '../../../entites/contract';
import {
  Subject
} from 'rxjs';
import {
  Msg
} from '../../../entites/msg';
import {
  takeUntil,
  take
} from 'rxjs/operators';
import {
  Budget
} from '../../../entites/budget';
import {
  NgxSpinnerService
} from 'ngx-spinner';
import {
  BankAccountService
} from '../../../services/bank-account.service';
import {
  BankAccount
} from '../../../entites/bank-account';
import {
  TableHeader
} from '../../../entites/table-headers';
import {
  TableBtn
} from '../../../entites/table-btn';
import {
  SwalPortalTargets, SwalComponent
} from '@sweetalert2/ngx-sweetalert2';
import {
  BudgetContract
} from '../../../entites/budget-contract';
import {
  Supplier
} from '../../../entites/supplier';
import { SupplierService } from '../../../services/supplier.service';

@Component({
  selector: 'app-contract-managment',
  templateUrl: './contract-management.component.html',
})
export class ContractManagementComponent implements OnInit, OnDestroy {
  @Input() isPopUp: boolean;
  @Input() newContract: Contract;
  @Input() editContract: Contract;
  @Input() budgets: Budget[];
  selectedBudgets: Budget[];
  supplier: Supplier = this.supplierService.getCurrentSupplier();
  contracts: Contract[];
  contractsToDisplay: any;
  @Output() addContractEvent = new EventEmitter < boolean > ();
  @Input() banks: BankAccount[];
  tableHeaders: TableHeader[];
  deleteBtn: TableBtn;
  editBtn: TableBtn;
  msg: Msg;
  isLoading: boolean;
  destroyed = new Subject();
  @ViewChild('contractEditing', {static: true }) private contractEditing: SwalComponent;
  constructor(public readonly swalTargets: SwalPortalTargets,
              private contractService: ContractService,
              private budgetService: BudgetService,
              private spinner: NgxSpinnerService, private bankService: BankAccountService,
              private supplierService: SupplierService) {}

  ngOnInit() {
    if (!this.budgets) {
      this.budgetService.getBySupplier()
        .pipe(takeUntil(this.destroyed))
        .subscribe(result => this.budgets = result);
    }
    if (!this.banks) {
      this.bankService.getBySupplier()
        .pipe(takeUntil(this.destroyed))
        .subscribe(result => this.banks = result);
    }
    if (!this.contracts) {
      this.getContractsBySupplier(true);
    }
    this.editContract = this.contractService.createEmptyEntity();
    if (!this.newContract) {
      this.newContract = this.contractService.createEmptyEntity();
      this.tableHeaders = this.contractService.getContractTableHeaders();
      this.editBtn = {
        text: 'ערוך',
        type: 'warning'
      };
      this.deleteBtn = {
        text: 'מחק',
        type: 'danger'
      };
    } else {
      this.selectedBudgets = [];
      this.newContract.budgetContract.forEach((ele) => {
        const budget: Budget = {
          id: ele.budgetId,
          name: '',
          customerId: ele.customerId,
          supplierId: ele.supplierId,
        };
        this.selectedBudgets.push(budget);
      });
    }
  }
  ngOnDestroy(): void {
    this.destroyed.next();
  }
  clearContract() {
    this.editContract = null;
  }
  getContractsBySupplier(toReload: boolean) {
    if (!this.isPopUp && toReload) {
      this.contractService.getContractsBySupplier()
        .pipe(takeUntil(this.destroyed))
        .subscribe(result => {
          this.contracts = result;
          this.contractsToDisplay = [];
          this.contracts.forEach((contract) => {
            const contractToDisplay = {
              id: contract.id,
              description: contract.description,
              bankAccountInFinance: contract.bankAccountInFinance,
              address: contract.address,
              budgetIds: '',
            };
            contract.budgetContract.map(x => contractToDisplay.budgetIds += x.budgetId + ',');
            contractToDisplay.budgetIds = contractToDisplay.budgetIds.substring(0 , contractToDisplay.budgetIds.length - 1);
            this.contractsToDisplay.push(contractToDisplay);
          });
        });
    }
  }
  addContract() {
    let sum = 0;
    this.newContract.budgetContract.forEach((ele) => {
      sum += parseFloat(ele.precent.toString());
      ele.precent = parseFloat(ele.precent.toString());
      ele.contractId = this.newContract.id;
    });
    if (sum === 100) {
      if (!this.contracts.find(x => x.id === this.newContract.id)) {
      this.isLoading = true;
      this.spinner.show();
      this.newContract.bankAccountInFinance = parseInt(this.newContract.bankAccountInFinance.toString() , 10);
      this.contractService.addContract(this.newContract)
        .pipe(takeUntil(this.destroyed))
        .subscribe(result => {
          this.msg = {
            text: result ? 'החוזה נוסף בהצלחה' : 'קרתה תקלה בעת הוספת החוזה',
            type: result ? 'success' : 'danger',
            dismissible: true
          };
          this.getContractsBySupplier(result);
          this.isLoading = false;
          this.spinner.hide();
          this.addContractEvent.emit(result);
        });
      } else {
        this.msg = {
          text: 'החוזה כבר קיים',
          type: 'danger',
          dismissible: false
        };
      }
    } else {
      this.msg = {
        text: 'סכום האחוזים בחוזה אינו יכול להיות שונה מ100',
        type: 'danger',
        dismissible: false
      };
    }
  }
  popUpEditWindow(contract: Contract) {

    this.editContract = contract;
    this.contractEditing.fire();
  }
  updateContract() {
    this.isLoading = true;
    this.spinner.show();
    this.editContract.bankAccountInFinance = parseInt(this.editContract.bankAccountInFinance.toString() , 10);
    this.contractService.editContract(this.editContract)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'החוזה עודכן בהצלחה' : 'קרתה תקלה בעת עדכון החוזה',
          type: result ? 'success' : 'danger',
          dismissible: true,
        };
        this.getContractsBySupplier(result);
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  deleteContract(contract: Contract) {
    this.isLoading = true;
    this.spinner.show();
    this.contractService.editContract(contract)
      .pipe(takeUntil(this.destroyed))
      .subscribe(result => {
        this.msg = {
          text: result ? 'החוזה נמחק בהצלחה' : 'קרתה תקלה בעת מחיקת החוזה',
          type: result ? 'success' : 'danger',
          dismissible: true,
        };
        this.getContractsBySupplier(result);
        this.isLoading = false;
        this.spinner.hide();
      });
  }
  checkRelationship() {
    let sum = 0;
    this.newContract.budgetContract.forEach((ele) => {
      if (ele.precent.toString().indexOf('.') !== ele.precent.toString().length) {
        sum += parseFloat(ele.precent.toString());
        if (sum > 100) {
          this.msg = {
            text: 'לא ניתן שסך הכל יחסי חוזה סעיף יהיה גדול מ100%',
            type: 'danger',
            dismissible: false
          };
          ele.precent = 0;
        }
      }
    });
  }
  addRelationship($event) {
    this.newContract.budgetContract.push({
      contractId: this.newContract.id,
      customerId: this.newContract.customerId,
      supplierId: this.newContract.supplierId,
      precent: 0,
      budgetId: $event.id
    });
  }
  removeRelationship($event) {
    this.newContract.budgetContract = this.newContract.budgetContract.filter(
      x => x.budgetId !== $event.value.id
    );
  }
}
