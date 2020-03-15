export interface BillingSummary {
    billingFrom: Date;
    billingTo: Date;
    totalFixed: number;
    totalChangable: number;
    totalOneTime: number;
    totalCredit: number;
    totalDebit: number;
    totalBillingBeforeTax: number;
    totalBilling: number;
    customerId: number;
    supplierId: string;
    rowId: number;
    isSent: boolean;
    dateOfValue: Date;
    totalExtraPayments: number;
    supplierClientNumber: number;
}
