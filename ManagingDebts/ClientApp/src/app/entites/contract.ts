import { BudgetContract } from './budget-contract';

export interface Contract {
    id: number;
    description: string;
    address?: string;
    bankAccountInFinance: number;
    customerId: number;
    supplierId: string;
    budgetContract?: BudgetContract[];
}
