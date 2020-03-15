using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class PrivateSupplierDataConfirmationService : IPrivateSupplierDataConfirmationService
    {
        private readonly ManagingDebtsContext context;
        public PrivateSupplierDataConfirmationService(ManagingDebtsContext context)
        {
            this.context = context;
        }

        public async Task<Dictionary<string, PrivateSupplierFileInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity)
        {
            var privateSupplierData = context.PrivateSupplierFileInfo.Where(x => x.GeneralRowId == summaryEntity.RowId)
                .AsNoTracking();
            return await createPrivateSupplierCodeDictionary(privateSupplierData.ToArray(), summaryEntity);
        }
        public async Task<bool> MatchPrivateSupplier(PrivateSupplierFileInfoEntity[] privates)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var result = await Task.Run(() =>
                    {
                        List<bool> results = new List<bool>();
                        for (int i = 0; i < privates.Length; i++)
                        {
                            results.Add(matchPrivateSupplierRecord(privates[i]));
                        }
                        return !results.SingleOrDefault(x => !x);
                    });
                    if (result)
                    {
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }

            }
        }
        private bool matchPrivateSupplierRecord(PrivateSupplierFileInfoEntity privateSupplier)
        {
            try
            {

                var data = context.PrivateSupplierFileInfo.SingleOrDefault(x => x.RowId == privateSupplier.RowId && x.GeneralRowId == privateSupplier.GeneralRowId &&x.CustomerId == privateSupplier.CustomerId 
                && x.SupplierId == privateSupplier.SupplierId);
                if (data != null)
                {
                    data.IsMatched = privateSupplier.IsMatched;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private PrivateSupplierFileInfoEntity createPrivateSupplierCodeEntity(PrivateSupplierFileInfo fileInfo)
        {
            return new PrivateSupplierFileInfoEntity
            {
                AmountAfterTax = fileInfo.AmountAfterTax,
                CustomerId = fileInfo.CustomerId,
                GeneralRowId = fileInfo.GeneralRowId,
                Invoice = fileInfo.Invoice,
                JournalEntryNumber = fileInfo.JournalEntryNumber,
                IsMatched = fileInfo.IsMatched,
                Contract = fileInfo.Contract,
                RowId = fileInfo.RowId,
                Amount = fileInfo.Amount,
                DateOfValue = fileInfo.DateOfValue,
                Description = fileInfo.Description,
                SupplierId = fileInfo.SupplierId,
                TaxRate = fileInfo.TaxRate,
            };
        }
        private async Task<PrivateSupplierFileInfoEntity[]> createPrivateSupplierCodeArray(PrivateSupplierFileInfo[] privates)
        {
            return await Task.Run(() =>
            {
                List<PrivateSupplierFileInfoEntity> infos = new List<PrivateSupplierFileInfoEntity>();
                for (int i = 0; i < privates.Length; i++)
                {
                    infos.Add(createPrivateSupplierCodeEntity(privates[i]));
                }
                return infos.ToArray();
            });

        }
        private PrivateSupplierFileInfoEntity createSummaryPrivateSupplierRow(IGrouping<int, PrivateSupplierFileInfo> row)
        {
            var defaultValues = row.First();
            decimal amountAfterTax = 0, amount=0;
            foreach(var contract in row)
            {
                amountAfterTax += contract.AmountAfterTax;
                amount = contract.Amount;
            }
            return new PrivateSupplierFileInfoEntity
            {
                AmountAfterTax = amountAfterTax,
                CustomerId = defaultValues.CustomerId,
                GeneralRowId = defaultValues.GeneralRowId,
                Invoice = 0,
                JournalEntryNumber = 0,
                Amount = amount,
                DateOfValue = defaultValues.DateOfValue,
                Description = "",
                SupplierId = defaultValues.SupplierId,
                TaxRate = 0,
                IsMatched = row.Where(x => !x.IsMatched).Count() == 0,
                Contract = row.Key,
                RowId = 0,
            };
        }
        private async Task<Dictionary<string, PrivateSupplierFileInfoEntity[]>> createPrivateSupplierCodeDictionary(PrivateSupplierFileInfo[] fileInfos, GeneralBillingSummaryEntity summaryEntity)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var data = fileInfos.GroupBy(x => x.Contract);
                Dictionary<string, PrivateSupplierFileInfoEntity[]> privateSupplierDic = new Dictionary<string, PrivateSupplierFileInfoEntity[]>();
                return await Task.Run(() => {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        var groupedRow = data.ElementAt(i);
                        privateSupplierDic.Add(JsonSerializer.Serialize(createSummaryPrivateSupplierRow(groupedRow), options), createPrivateSupplierCodeArray(groupedRow.ToArray()).Result);
                    }
                    return privateSupplierDic;
                });
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
