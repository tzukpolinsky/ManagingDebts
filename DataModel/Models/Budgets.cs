using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class Budgets
    {
        public Budgets()
        {
            BudgetsContracts = new HashSet<BudgetsContracts>();
        }

        public long BudgetId { get; set; }
        public string BudgetName { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }

        public virtual Suppliers Suppliers { get; set; }
        public virtual ICollection<BudgetsContracts> BudgetsContracts { get; set; }
    }
}
