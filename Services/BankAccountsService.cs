using Entities;
using ManagingDebts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BankAccountsService : IBankAccountsService
    {
        private readonly ManagingDebtsContext context;

        public BankAccountsService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public BankAccountEntity[] GetBySupplier(SupplierEntity supplier)
        {
            try
            {
                var banks = context.BankAccounts.Where(x => x.CustomerId == supplier.CustomerId && x.SupplierId == supplier.Id).Select(x=> new BankAccountEntity
                { BankAccountInFinance =x.BankAccountInFinance,CustomerId = x.CustomerId, SupplierId = x.SupplierId });
                return banks != null ? banks.ToArray() : null;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public bool Delete(BankAccountEntity bankAccountEntity)
        {
            using (var transaction = context.Database.BeginTransaction()) { 
                try
                {
                
                    var contracts =context.Contracts.Where(x => x.BankAccountInFinance == bankAccountEntity.BankAccountInFinance);
                    foreach (var contract in contracts)
                    {
                        contract.BankAccountInFinance = 0;
                    }
                    context.SaveChanges();
                    context.BankAccounts.Remove(new BankAccounts
                    {
                        BankAccountInFinance = bankAccountEntity.BankAccountInFinance,
                        CustomerId = bankAccountEntity.CustomerId,
                        SupplierId = bankAccountEntity.SupplierId
                    });
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
        public bool Add(BankAccountEntity bankAccountEntity)
        {
            try
            {
                context.BankAccounts.Add(new BankAccounts
                {
                    BankAccountInFinance = bankAccountEntity.BankAccountInFinance,
                    CustomerId = bankAccountEntity.CustomerId,
                    SupplierId = bankAccountEntity.SupplierId
                });
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
