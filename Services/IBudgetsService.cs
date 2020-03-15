using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBudgetsService
    {
        BudgetEntity[] GetBySupplier(SupplierEntity supplier);
        bool Add(BudgetEntity budgetEntity);
        bool AddMany(BudgetEntity[] budgets);
        bool Delete(BudgetEntity budgetEntity);
        bool Edit(BudgetEntity budgetEntity);
        Task<BudgetEntity[]> ConvertFileToEntites(IFormFile file);
        Task<BudgetEntity[]> GetBudgetsFromFinance(BudgetEntity customer);
        Task<bool> AddRelationshipByFile(FileEntity fileEntity);

    }
}
