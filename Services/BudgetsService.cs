using Entities;
using ManagingDebts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BudgetsService : IBudgetsService
    {
        private readonly ManagingDebtsContext context;
        //private ILogger logger;
        public BudgetsService(ManagingDebtsContext context/*, ILogger logger*/)
        {
            this.context = context;
            //this.logger = logger;
        }
        public async Task<bool> AddRelationshipByFile(FileEntity fileEntity)
        {
            var pck = new ExcelPackage();
            pck.Load(fileEntity.File.OpenReadStream());
            try
            {
                SupplierEntity supplier = new SupplierEntity
                {
                    Id = fileEntity.SupplierId,
                    CustomerId = fileEntity.CustomerId
                };
                var result = await Task.Run(() =>
                {
                    var excel = pck.Workbook.Worksheets.First();
                    var budgets = new BudgetsService(context).GetBySupplier(supplier);
                    var contracts = new ContractsService(context).GetBySupplier(supplier).Result;
                    var relationship = new BudgetContractService(context).GetRelationshipBySupplier(supplier);
                    for (int rowNum = 2; rowNum <= excel.Dimension.End.Row; rowNum++)
                    {
                        if (!string.IsNullOrEmpty(excel.Cells[rowNum, 1].Text))
                        {
                            handleRelationShipsExcel(excel, rowNum, contracts, budgets, relationship, supplier);
                        }
                        else
                        {
                            break;
                        }
                    }
                    return true;
                });
                pck.Dispose();
                await context.SaveChangesAsync();
                return result;
            }
            catch (Exception e)
            {
                pck.Dispose();
                throw e;
            }
        }
        public BudgetEntity[] GetBySupplier(SupplierEntity supplier)
        {
            var data = context.Budgets.Where(x=>x.CustomerId == supplier.CustomerId && x.SupplierId == supplier.Id).Select(x=>
                new BudgetEntity { Id = x.BudgetId, Name = x.BudgetName, CustomerId = x.CustomerId,SupplierId =x.SupplierId }).AsNoTracking();
            return data != null ? data.ToArray() : null;
        }
        public async Task<BudgetEntity[]> GetBudgetsFromFinance(BudgetEntity customer)
        {
            //TODO: connect finance serivce , stuck at orit responbilty
            return null;
        }
        public async Task<BudgetEntity[]> ConvertFileToEntites(IFormFile file)
        {
            var pck = new OfficeOpenXml.ExcelPackage();
            pck.Load(file.OpenReadStream());
            OfficeOpenXml.ExcelWorksheet excel = pck.Workbook.Worksheets[0];
            return await Task.Run(() =>
            {
                List<BudgetEntity> budgets = new List<BudgetEntity>();
                for (int rowNum = 2; rowNum <= excel.Dimension.End.Row; rowNum++)
                {
                        var result = ConvertExcelRowToBudget(excel, rowNum);
                        if (result!=null)
                        {
                            budgets.Add(result);
                        }
                }
                return budgets.ToArray();
            });
        }
        public bool Add(BudgetEntity budgetEntity)
        {
            try
            {
                if (budgetEntity != null)
                {
                    context.Budgets.Add(new Budgets
                    {
                        BudgetId = budgetEntity.Id,
                        BudgetName = budgetEntity.Name,
                        CustomerId = budgetEntity.CustomerId,
                        SupplierId =budgetEntity.SupplierId
                    });
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool AddMany(BudgetEntity[] budgets)
        {
            try
            {
                if (budgets != null)
                {
                    context.Budgets.AddRange(budgets.Select(x=>new Budgets { 
                    BudgetId = x.Id,BudgetName = x.Name, CustomerId=x.CustomerId,SupplierId = x.SupplierId
                    }));
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool Delete(BudgetEntity budgetEntity)
        {
            try
            {
                if (budgetEntity != null )
                {
                    context.BudgetsContracts.RemoveRange(new BudgetsContracts { BudgetId = budgetEntity.Id, SupplierId = budgetEntity.SupplierId, CustomerId = budgetEntity.CustomerId });
                    context.Budgets.Remove(new Budgets
                    {
                        BudgetId = budgetEntity.Id,
                        BudgetName = budgetEntity.Name,
                        CustomerId = budgetEntity.CustomerId,
                        SupplierId = budgetEntity.SupplierId
                    });
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool Edit(BudgetEntity budgetEntity)
        {
            try
            {
                if (budgetEntity != null)
                {
                    var oldBudget = context.Budgets.Find(budgetEntity.Id, budgetEntity.CustomerId,budgetEntity.SupplierId);
                    if (oldBudget!=null)
                    {
                        oldBudget.BudgetName = budgetEntity.Name;
                        context.SaveChanges();
                        return true;
                    } 
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private BudgetEntity ConvertExcelRowToBudget(OfficeOpenXml.ExcelWorksheet sheet, int rowNum)
        {
            int budgetId = 0;
            if (sheet.Cells[rowNum, 2].Text.Trim().Length == 10)
            {
                if (int.TryParse(sheet.Cells[rowNum, 2].Text.Trim(),out budgetId))
                {
                   return new BudgetEntity { 
                    Id = budgetId,
                    Name = sheet.Cells[rowNum, 3].Text.Trim(),
                    };
                }
            }
            return null;
        }
        private bool handleRelationShipsExcel(ExcelWorksheet sheet, int rowNum, ContractEntity[] contracts, BudgetEntity[] budgets, BudgetContractEntity[] budgetContracts, SupplierEntity supplier)
        {
            int contractId = 0;
            string contractValue = sheet.Cells[rowNum, 1].Text.Trim().Replace("-", "");
            if (int.TryParse(contractValue, out contractId))
            {
                var contract = contracts.SingleOrDefault(x => x.Id == contractId);
                if (contract == null)
                {
                    int bankAccount = 0;
                    int.TryParse(sheet.Cells[rowNum, 4].Text.Trim(), out bankAccount);
                    context.Contracts.Add(new Contracts
                    {
                        ContractId = contractId,
                        ContractAddress = "",
                        ContractDescription = "",
                        CustomerId = supplier.CustomerId,
                        SupplierId = supplier.Id,
                        BankAccountInFinance = bankAccount,
                    });
                }
                int budgetId = 0;
                if (int.TryParse(sheet.Cells[rowNum, 2].Text.Trim(), out budgetId))
                {
                    var budget = budgets.SingleOrDefault(x => x.Id == budgetId);
                    if (budget == null)
                    {
                        context.Budgets.Add(new Budgets
                        {
                            BudgetId = budgetId,
                            SupplierId = supplier.Id,
                            CustomerId = supplier.CustomerId,
                            BudgetName = "",
                        });
                    }

                    var budgetContract = budgetContracts.Where(x => x.BudgetId == budgetId && x.ContractId == contractId);
                    if (budgetContract == null)
                    {
                        context.BudgetsContracts.Add(new BudgetsContracts
                        {
                            BudgetId = budgetId,
                            ContractId = contractId,
                            CustomerId = supplier.CustomerId,
                            SupplierId = supplier.Id,
                            BudgetPrecent = 100,
                        });

                    }
                    else
                    {
                        context.BudgetsContracts.RemoveRange(budgetContract.Select(x => new BudgetsContracts
                        {
                            ContractId = x.ContractId,
                            BudgetId = x.BudgetId,
                            SupplierId = x.SupplierId,
                            CustomerId = x.CustomerId,
                            BudgetPrecent = x.Precent
                        }));
                        context.BudgetsContracts.Add(new BudgetsContracts
                        {
                            BudgetId = budgetId,
                            ContractId = contractId,
                            CustomerId = supplier.CustomerId,
                            SupplierId = supplier.Id,
                            BudgetPrecent = 100,
                        });
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
