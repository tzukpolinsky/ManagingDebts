import { Contract } from './contract';
import { Customer } from './customer';

export interface BankAccount {
    bankAccountInFinance: number;
    customerId: number;
    supplierId: string;
    contracts?: Contract[];
}
