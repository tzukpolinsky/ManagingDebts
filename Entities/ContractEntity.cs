using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class ContractEntity
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public long BankAccountInFinance { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
        public BudgetContractEntity[] BudgetContract { get; set; }
    }
}
