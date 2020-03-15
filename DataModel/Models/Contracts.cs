using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class Contracts
    {
        public Contracts()
        {
            BudgetsContracts = new HashSet<BudgetsContracts>();
        }

        public int ContractId { get; set; }
        public string ContractDescription { get; set; }
        public long BankAccountInFinance { get; set; }
        public int CustomerId { get; set; }
        public string ContractAddress { get; set; }
        public string SupplierId { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Suppliers Suppliers { get; set; }
        public virtual ICollection<BudgetsContracts> BudgetsContracts { get; set; }
    }
}
