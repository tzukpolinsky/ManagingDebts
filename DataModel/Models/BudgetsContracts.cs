using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class BudgetsContracts
    {
        public long BudgetId { get; set; }
        public int ContractId { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
        public double BudgetPrecent { get; set; }

        public virtual Budgets Budgets { get; set; }
        public virtual Contracts Contracts { get; set; }
    }
}
