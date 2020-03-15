using Entities;
using ManagingDebts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IBankAccountsService
    {
        BankAccountEntity[] GetBySupplier(SupplierEntity supplier);
        bool Delete(BankAccountEntity bankAccountEntity);
        bool Add(BankAccountEntity bankAccountEntity);
    }
}
