using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IContractsService
    {
        Task<ContractEntity[]> GetBySupplier(SupplierEntity supplier);
        bool Edit(ContractEntity contract);
        bool Delete(ContractEntity contract);
        bool Add(ContractEntity contract);

    }
}
