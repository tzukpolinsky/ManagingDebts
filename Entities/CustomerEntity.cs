using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class CustomerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public SupplierEntity[] Suppliers{ get; set; }
        public BudgetEntity[] Budgets { get; set; }
        public ContractEntity[] Contracts { get; set; }
        public BankAccountEntity[] bankAccounts { get; set; }
    }
}
