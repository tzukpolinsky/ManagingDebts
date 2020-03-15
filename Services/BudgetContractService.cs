using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class BudgetContractService: IBudgetContractService
    {
        private readonly ManagingDebtsContext context;

        public BudgetContractService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public BudgetContractEntity[] GetRelationByContract(ContractEntity contract)
        {
            var result = context.BudgetsContracts.Where(x => x.CustomerId == contract.CustomerId && x.SupplierId == contract.SupplierId && x.ContractId == contract.Id).AsNoTracking().Select(x => new BudgetContractEntity
            {
                ContractId = x.ContractId,
                BudgetId = x.BudgetId,
                Precent = x.BudgetPrecent,
                CustomerId = x.CustomerId,
                SupplierId = x.SupplierId
            }).ToArray();
            return result;
        }
        public BudgetContractEntity[] GetRelationshipBySupplier(SupplierEntity supplier)
        {
            var result = context.BudgetsContracts.Where(x => x.CustomerId == supplier.CustomerId && x.SupplierId == supplier.Id).AsNoTracking().Select(x => new BudgetContractEntity { 
            ContractId = x.ContractId,
            BudgetId = x.BudgetId,
            Precent =x.BudgetPrecent,
            CustomerId = x.CustomerId,
            SupplierId = x.SupplierId
            }).ToArray();
            return result;
        }
    }
}
