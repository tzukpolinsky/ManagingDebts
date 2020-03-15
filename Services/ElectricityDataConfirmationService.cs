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
    public class ElectricityDataConfirmtionService: IElectricityDataConfirmationService
    {
        private readonly ManagingDebtsContext context;
        public ElectricityDataConfirmtionService(ManagingDebtsContext context)
        {
            this.context = context;
        }

        public async Task<Dictionary<string, ElectricityInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity)
        {
            var electricityData = context.ElectricityFileInfo.Where(x => x.GeneralRowId == summaryEntity.RowId)
                .AsNoTracking();
            return await createElectricityCodeDictionary(electricityData.ToArray(), summaryEntity);
        }
        public async Task<bool> MatchElectricity(ElectricityInfoEntity[] electricities)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var result = await Task.Run(() =>
                    {
                        List<bool> results = new List<bool>();
                        for (int i = 0; i < electricities.Length; i++)
                        {
                            results.Add(matchElectricityRecord(electricities[i]));
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
        private bool matchElectricityRecord(ElectricityInfoEntity electricity)
        {
            try
            {

                var data = context.ElectricityFileInfo.SingleOrDefault(x => x.RowId == electricity.RowId && x.GeneralRowId == electricity.GeneralRowId &&x.CustomerId == electricity.CustomerId);
                if (data != null)
                {
                    data.IsMatched = electricity.IsMatched;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private ElectricityInfoEntity createElectricityCodeEntity(ElectricityFileInfo electricity)
        {
            return new ElectricityInfoEntity
            {
                AmountAfterTax = electricity.Amount,
                BankAccount = electricity.BankAccount,
                BankAccountType = electricity.BankAccountType,
                Branch = electricity.BankBranch,
                Bank = electricity.BankCode,
                BillCreatingDate = electricity.BillCreatingDate,
                ConsumerAddress = electricity.ConsumerAddress,
                ConsumerName = electricity.ConsumerName,
                ConsumerNumber = electricity.ConsumerNumber,
                CustomerId = electricity.CustomerId,
                GeneralRowId = electricity.GeneralRowId,
                Invoice = electricity.Invoice,
                JournalEntryNumber = electricity.JournalEntryNumber,
                MonthOfLastInvoice = electricity.MonthOfLastInvoice,
                NumberOfCreditDays = electricity.NumberOfCreditDays,
                PaymentDate = electricity.PaymentDate,
                YearOfLastInvoice = electricity.YearOfLastInvoice,
                IsMatched = electricity.IsMatched,
                Contract = electricity.Contract,
                RowId = electricity.RowId,
            };
        }
        private async Task<ElectricityInfoEntity[]> createElectricityCodeArray(ElectricityFileInfo[] electricities)
        {
            return await Task.Run(() =>
            {
                List<ElectricityInfoEntity> infos = new List<ElectricityInfoEntity>();
                for (int i = 0; i < electricities.Length; i++)
                {
                    infos.Add(createElectricityCodeEntity(electricities[i]));
                }
                return infos.ToArray();
            });

        }
        private ElectricityInfoEntity createSummaryElectricityRow(IGrouping<int, ElectricityFileInfo> row)
        {
            var defaultValues = row.First();
            var maxPaymentDate = row.Max(x => x.PaymentDate);
            return new ElectricityInfoEntity
            {
                AmountAfterTax = row.Sum(x => x.Amount),
                BankAccount = defaultValues.BankAccount,
                BankAccountType = defaultValues.BankAccountType,
                Branch = defaultValues.BankBranch,
                Bank = defaultValues.BankCode,
                BillCreatingDate = row.Min(x => x.BillCreatingDate),
                ConsumerAddress = defaultValues.ConsumerAddress,
                ConsumerName = defaultValues.ConsumerName,
                ConsumerNumber = defaultValues.ConsumerNumber,
                CustomerId = defaultValues.CustomerId,
                GeneralRowId = defaultValues.GeneralRowId,
                Invoice = 0,
                JournalEntryNumber = 0,
                MonthOfLastInvoice = maxPaymentDate.Month,
                NumberOfCreditDays = defaultValues.NumberOfCreditDays,
                PaymentDate = maxPaymentDate,
                YearOfLastInvoice = maxPaymentDate.Year,
                IsMatched = row.Where(x => !x.IsMatched).Count() == 0,
                Contract = row.Key,
                RowId = 0,
            };
        }
        private async Task<Dictionary<string, ElectricityInfoEntity[]>> createElectricityCodeDictionary(ElectricityFileInfo[] electricities, GeneralBillingSummaryEntity summaryEntity)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var data = electricities.GroupBy(x => x.Contract);
                Dictionary<string, ElectricityInfoEntity[]> electricDic = new Dictionary<string, ElectricityInfoEntity[]>();
                return await Task.Run(() => {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        var groupedRow = data.ElementAt(i);
                        electricDic.Add(JsonSerializer.Serialize(createSummaryElectricityRow(groupedRow), options), createElectricityCodeArray(groupedRow.ToArray()).Result);
                    }
                    return electricDic;
                });
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
