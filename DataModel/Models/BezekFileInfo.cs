using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class BezekFileInfo
    {
        public int CustomerId { get; set; }
        public string ClientNumber { get; set; }
        public int PayerNumberBezek { get; set; }
        public string DepartmentNumber { get; set; }
        public int SubscriptionNumber { get; set; }
        public DateTime? StartDateBilling { get; set; }
        public DateTime? EndDateBilling { get; set; }
        public string BillingType { get; set; }
        public string BillingDescription { get; set; }
        public decimal BillingAmount { get; set; }
        public int ConsumptionAmount { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public string OriginalClient { get; set; }
        public int OriginalPayer { get; set; }
        public double DiscountPrecent { get; set; }
        public string CallTime { get; set; }
        public int? CallsAmount { get; set; }
        public decimal CallRate { get; set; }
        public string FreeTimeUsage { get; set; }
        public string TimePeriodText { get; set; }
        public string ServiceType { get; set; }
        public string SecondaryServiceType { get; set; }
        public string HebServiceType { get; set; }
        public string FreeTimeUsageSupplier { get; set; }
        public int GeneralRowId { get; set; }
        public bool IsMatched { get; set; }
        public int RowId { get; set; }
        public int? JournalEntryNumber { get; set; }
        public int TaxRate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal BillingAmountAfterTax { get; set; }

        public virtual GeneralBillingSummary GeneralRow { get; set; }
    }
}
