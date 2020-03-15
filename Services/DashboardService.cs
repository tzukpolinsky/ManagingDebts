using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ManagingDebtsContext context;

        public DashboardService(ManagingDebtsContext context)
        {
            this.context = context;
        }

        public ChartEntity[] GetContractChartInfo(SupplierEntity supplier)
        {
            switch (Convert.ToInt32(supplier.Id))
            {
                case (int)Enums.Suppliers.Bezek:
                    return getBezekContractInfo(supplier);
                case (int)Enums.Suppliers.Electricity:
                    return getElectricityContractInfo(supplier);
                default:
                    return getPrivateSupplierContractInfo(supplier);
            }
        }
        private ChartEntity[] getBezekContractInfo(SupplierEntity supplier)
        {
            return context.BezekFileInfo.Where(x => x.CustomerId == supplier.CustomerId).AsNoTracking().ToArray().GroupBy(x => x.SubscriptionNumber).Select(x =>
              new ChartEntity
              {
                  TopicId = x.Key,
                  Amount = x.Sum( y => y.BillingAmountAfterTax)
              }).ToArray();
        }
        private ChartEntity[] getElectricityContractInfo(SupplierEntity supplier)
        {
            return context.ElectricityFileInfo.Where(x => x.CustomerId == supplier.CustomerId).AsNoTracking().ToArray().GroupBy(x => x.Contract).Select(x =>
              new ChartEntity
              {
                  TopicId = x.Key,
                  Amount = x.Sum(y => y.Amount)
              }).ToArray();
        }
        private ChartEntity[] getPrivateSupplierContractInfo(SupplierEntity supplier)
        {
            return context.PrivateSupplierFileInfo.Where(x => x.CustomerId == supplier.CustomerId).AsNoTracking().ToArray().GroupBy(x => x.Contract).Select(x =>
              new ChartEntity
              {
                  TopicId = x.Key,
                  Amount = x.Sum(y => y.AmountAfterTax)
              }).ToArray();
        }
        public ChartEntity[] GetBudgetChartInfo(SupplierEntity supplier)
        {
            switch (Convert.ToInt32(supplier.Id))
            {
                case (int)Enums.Suppliers.Bezek:
                    return getBudgetInfo(supplier,getBezekContractInfo(supplier));
                case (int)Enums.Suppliers.Electricity:
                    return getBudgetInfo(supplier, getElectricityContractInfo(supplier));
                default:
                    return getBudgetInfo(supplier, getPrivateSupplierContractInfo(supplier));
            }
        }
        private ChartEntity[] getBudgetInfo(SupplierEntity supplier,ChartEntity[] contracts)
        {
            Dictionary<long, decimal> chartEntitiesDictionary = new Dictionary<long, decimal>();
            var budegtsContracts = new BudgetContractService(context).GetRelationshipBySupplier(supplier);
            decimal amount = 0;
            foreach (var contract in contracts)
            {
                var budget = budegtsContracts.FirstOrDefault(x => x.ContractId == contract.TopicId);
                if (budget != null)
                {
                    if (chartEntitiesDictionary.TryGetValue(budget.BudgetId,out amount))
                    {
                        chartEntitiesDictionary[budget.BudgetId] += amount *Convert.ToDecimal(budget.Precent) / 100;
                    }
                    else
                    {
                        chartEntitiesDictionary.Add(budget.BudgetId, contract.Amount * Convert.ToDecimal(budget.Precent) / 100);
                    }
                }
            }
            return chartEntitiesDictionary.Select(x => new ChartEntity { TopicId = x.Key, Amount = x.Value }).ToArray();
        }
    }
}
