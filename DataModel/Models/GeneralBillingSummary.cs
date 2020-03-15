using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class GeneralBillingSummary
    {
        public GeneralBillingSummary()
        {
            BezekFileInfo = new HashSet<BezekFileInfo>();
            ElectricityFileInfo = new HashSet<ElectricityFileInfo>();
            PrivateSupplierFileInfo = new HashSet<PrivateSupplierFileInfo>();
        }

        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
        public int SupplierPayerId { get; set; }
        public int SupplierClientNumber { get; set; }
        public DateTime BillFromDate { get; set; }
        public DateTime BillToDate { get; set; }
        public decimal TotalFixedBilling { get; set; }
        public decimal TotalChangableBilling { get; set; }
        public decimal TotalOneTimeBilling { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalInvoiceBeforeTax { get; set; }
        public decimal TotalInvoice { get; set; }
        public int RowId { get; set; }
        public bool Sent { get; set; }
        public DateTime DateOfValue { get; set; }
        public decimal TotalExtraPayments { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Suppliers Suppliers { get; set; }
        public virtual ICollection<BezekFileInfo> BezekFileInfo { get; set; }
        public virtual ICollection<ElectricityFileInfo> ElectricityFileInfo { get; set; }
        public virtual ICollection<PrivateSupplierFileInfo> PrivateSupplierFileInfo { get; set; }
    }
}
