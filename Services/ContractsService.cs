using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ContractsService : IContractsService
    {
        private readonly ManagingDebtsContext context;

        public ContractsService(ManagingDebtsContext context)
        {
            this.context = context;
        }

        public async Task<ContractEntity[]> GetBySupplier(SupplierEntity supplier)
        {
            var data = context.Contracts.Where(x => x.CustomerId == supplier.CustomerId && x.SupplierId == supplier.Id).AsNoTracking();
            return await CreateContractArray(data);
        }
        public bool Edit(ContractEntity contract)
        {
            try
            {
                var oldContract = context.Contracts.SingleOrDefault(x => x.ContractId == contract.Id && x.CustomerId == contract.CustomerId && x.SupplierId == contract.SupplierId);
                if (oldContract != null)
                {

                    oldContract.ContractDescription = contract.Description;
                    oldContract.ContractAddress = contract.Address;
                    oldContract.BankAccountInFinance = contract.BankAccountInFinance;
                    context.SaveChanges();
                    context.BudgetsContracts.RemoveRange(context.BudgetsContracts.Where(x => x.ContractId == contract.Id && x.CustomerId == contract.CustomerId && x.SupplierId == contract.SupplierId));

                    context.SaveChanges();
                    context.BudgetsContracts.AddRange(contract.BudgetContract.Select(x => new BudgetsContracts
                    {
                        BudgetId = x.BudgetId,
                        ContractId = contract.Id,
                        CustomerId = x.CustomerId,
                        SupplierId = x.SupplierId,
                        BudgetPrecent = x.Precent

                    }));
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public bool Delete(ContractEntity contract)
        {
            try
            {
                context.BudgetsContracts.RemoveRange(new BudgetsContracts { ContractId = contract.Id, CustomerId = contract.CustomerId, SupplierId = contract.SupplierId });
                context.Contracts.Remove(new Contracts { CustomerId = contract.CustomerId, SupplierId = contract.SupplierId, ContractId = contract.Id });
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }


        }
        public bool Add(ContractEntity contract)
        {
            try
            {
                context.Contracts.Add(new Contracts
                {
                    BankAccountInFinance = contract.BankAccountInFinance,
                    ContractAddress = contract.Address,
                    ContractDescription = contract.Description,
                    ContractId = contract.Id,
                    CustomerId = contract.CustomerId,
                    SupplierId = contract.SupplierId
                });
                double sum = 0;
                for (int i = 0; i < contract.BudgetContract.Length; i++)
                {
                    sum += contract.BudgetContract[i].Precent;
                    context.BudgetsContracts.Add(new BudgetsContracts
                    {
                        ContractId = contract.Id,
                        CustomerId = contract.CustomerId,
                        SupplierId = contract.SupplierId,
                        BudgetId = contract.BudgetContract[i].BudgetId,
                        BudgetPrecent = contract.BudgetContract[i].Precent
                    });
                }
                if (sum != 100)
                {
                    return false;
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        private ContractEntity CreateEntityAsync(Contracts dataContract)
        {
            var newContract = new ContractEntity()
            {
                BankAccountInFinance = dataContract.BankAccountInFinance,
                Description = dataContract.ContractDescription,
                Id = dataContract.ContractId,
                CustomerId = dataContract.CustomerId,
                Address = dataContract.ContractAddress,
                SupplierId = dataContract.SupplierId
            };
            newContract.BudgetContract = new BudgetContractService(context).GetRelationByContract(newContract);
            return newContract;
        }
        private async Task<ContractEntity[]> CreateContractArray(IEnumerable<Contracts> dataContract)
        {

            return await Task.Run(() =>
            {
                List<ContractEntity> contractEntities = new List<ContractEntity>();
                foreach (var contract in dataContract)
                {
                    contractEntities.Add(CreateEntityAsync(contract));
                }
                return contractEntities.ToArray();
            });
        }

    }
}
