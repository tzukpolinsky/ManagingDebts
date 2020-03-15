import { Contract } from './contract';

export interface Budget {
    id: number;
    customerId: number;
    name: string;
    contracts?: Contract[];
    supplierId: string;
}
