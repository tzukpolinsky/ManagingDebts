using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class BankAccounts
    {
        public long BankAccountInFinance { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }

        public virtual Suppliers Suppliers { get; set; }
    }
}
