using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class BudgetContractEntity
    {
        public int ContractId { get; set; }
        public long BudgetId { get; set; }
        public double Precent { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
    }
}
