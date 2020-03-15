using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IBudgetContractService
    {
        BudgetContractEntity[] GetRelationshipBySupplier(SupplierEntity supplier);
        BudgetContractEntity[] GetRelationByContract(ContractEntity contract);
    }
}
