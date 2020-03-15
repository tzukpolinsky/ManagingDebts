export interface Supplier {
    id: string;
    name: string;
    customerId?: number;
    isEnable?: boolean;
    supplierNumberInFinance: number;
    withBanks: boolean;
    pkudatYomanNumber?: number;
}
