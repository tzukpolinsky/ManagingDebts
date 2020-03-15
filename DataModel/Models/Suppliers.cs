using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class Suppliers
    {
        public Suppliers()
        {
            BankAccounts = new HashSet<BankAccounts>();
            Budgets = new HashSet<Budgets>();
            Contracts = new HashSet<Contracts>();
            GeneralBillingSummary = new HashSet<GeneralBillingSummary>();
        }

        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public bool SupplierEnabled { get; set; }
        public bool SupplierWithBanks { get; set; }
        public int SupplierPkudatYomanNumber { get; set; }
        public int SupplierCustomerId { get; set; }
        public long SupplierNumberInFinance { get; set; }

        public virtual ICollection<BankAccounts> BankAccounts { get; set; }
        public virtual ICollection<Budgets> Budgets { get; set; }
        public virtual ICollection<Contracts> Contracts { get; set; }
        public virtual ICollection<GeneralBillingSummary> GeneralBillingSummary { get; set; }
    }
}
