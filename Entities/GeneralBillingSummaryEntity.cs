using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class GeneralBillingSummaryEntity
    {
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
        //public int SupplierPayerId { get; set; }
        public int SupplierClientNumber { get; set; }
        public DateTime BillingFrom { get; set; }
        public DateTime BillingTo { get; set; }
        public int RowId { get; set; }
        public decimal TotalFixed { get; set; }
        public decimal TotalChangable { get; set; }
        public decimal TotalOneTime { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalBillingBeforeTax { get; set; }
        public decimal TotalBilling { get; set; }
        public bool IsSent { get; set; }
        public DateTime DateOfValue { get; set; }
        public decimal TotalExtraPayments { get; set; }
    }
}
