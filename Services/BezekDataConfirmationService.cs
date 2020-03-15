using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Services
{
    public class BezekDataConfirmationService : IBezekDataConfirmationService
    {
        private readonly ManagingDebtsContext context;

        public BezekDataConfirmationService(ManagingDebtsContext context)
        {
            this.context = context;
        }

        public async Task<Dictionary<string, BezekInfoEntity[]>> GetDataByDate(GeneralBillingSummaryEntity summaryEntity)
        {

            switch (Convert.ToInt32(summaryEntity.SupplierId))
            {
                case (int)Enums.Suppliers.Bezek:
                    var data = context.BezekFileInfo.Where(x => x.CustomerId == summaryEntity.CustomerId && x.StartDateBilling >= summaryEntity.BillingFrom && x.EndDateBilling <= summaryEntity.BillingTo)
                .AsNoTracking();
                    return await createBezekCodeDictionary(data.ToArray(), summaryEntity);
            }
            return null;
        }
        public async Task<Dictionary<string, BezekInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity)
        {
                 var data = context.BezekFileInfo.Where(x => x.GeneralRowId == summaryEntity.RowId)
                .AsNoTracking();
                return await createBezekCodeDictionary(data.ToArray(), summaryEntity);
        }
        public async Task<bool> MatchBezek(BezekInfoEntity[] bezeks)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var result = await Task.Run(() =>
                    {
                        List<bool> results = new List<bool>();
                        for (int i = 0; i < bezeks.Length; i++)
                        {
                            results.Add(matchBezekRecord(bezeks[i]));
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
        
        private bool matchBezekRecord(BezekInfoEntity bezek)
        {
            try
            {
                
                var data = context.BezekFileInfo.SingleOrDefault(x => x.RowId == bezek.RowId && x.GeneralRowId == bezek.GeneralSummaryRowId && x.CustomerId == bezek.CustomerId);
                if (data != null)
                {
                    data.IsMatched = bezek.IsMatched;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
       
        private async Task<BezekInfoEntity> createBezekCodeEntity(BezekFileInfo bezek)
        {
            return new BezekInfoEntity
            {
                Amount = bezek.BillingAmount,
                AmountAfterTax = bezek.BillingAmountAfterTax,
                CallRate = bezek.CallRate,
                CallsAmount = bezek.CallsAmount,
                CallTime = bezek.CallTime,
                DepartmentNumber = bezek.DepartmentNumber,
                ClientNumber = bezek.ClientNumber,
                ConsumptionAmount = bezek.ConsumptionAmount,
                Contract = bezek.SubscriptionNumber,
                Description = bezek.BillingDescription,
                DiscountPrecent = bezek.DiscountPrecent,
                EndDate = bezek.EndDateBilling,
                FreeTimeUsage = bezek.FreeTimeUsage,
                FreeTimeUsageSupplier = bezek.FreeTimeUsageSupplier,
                GeneralSummaryRowId = bezek.GeneralRowId,
                HebServiceType = bezek.HebServiceType,
                IsMatched = bezek.IsMatched,
                MonthlyRate = bezek.MonthlyRate,
                OriginalClient = bezek.OriginalClient,
                OriginalPayer = bezek.OriginalPayer,
                PayerNumberBezek = bezek.PayerNumberBezek,
                PriceBeforeDiscount = bezek.PriceBeforeDiscount,
                SecondaryServiceType = bezek.SecondaryServiceType,
                ServiceType = bezek.ServiceType,
                StartDate = bezek.StartDateBilling,
                TimePeriodText = bezek.TimePeriodText,
                Type = bezek.BillingType,
                RowId = bezek.RowId,
                TaxRate = bezek.TaxRate,
                CustomerId = bezek.CustomerId
            };
        }
       
        private BezekInfoEntity[] createBezekCodeArray(BezekFileInfo[] bezeks)
        {
            List<Task<BezekInfoEntity>> tasks = new List<Task<BezekInfoEntity>>();
            for (int i = 0; i < bezeks.Length; i++)
            {
                tasks.Add(createBezekCodeEntity(bezeks[i]));
            }
            return Task.WhenAll(tasks).Result;
        }
       
        private BezekInfoEntity createSummaryBezekRow(IGrouping<int, BezekFileInfo> row)
        {
            SharedService validationsService = new SharedService();
            var defaultValues = row.First();
            var contractSummary = new BezekFileInfo();
            long callTimeSum=0, freeTimeSum=0, freeSupplierTimeSum = 0;
            decimal callRateSum=0, monthlyRateSum = 0;
            double discountPresent = 0;
            int amountOfContracts=row.Count();
            for (int i = 0; i < amountOfContracts; i++)
            {
                var currentContract = row.ElementAt(i);
                contractSummary.BillingAmount += currentContract.BillingAmount;
                contractSummary.BillingAmountAfterTax += currentContract.BillingAmountAfterTax;
                callRateSum += currentContract.CallRate;
                contractSummary.CallsAmount += currentContract.CallsAmount;
                callTimeSum += validationsService.GetCurrectTimeSpanTicksByString(currentContract.CallTime);
                freeTimeSum += validationsService.GetCurrectTimeSpanTicksByString(currentContract.FreeTimeUsage);
                freeSupplierTimeSum += validationsService.GetCurrectTimeSpanTicksByString(currentContract.FreeTimeUsageSupplier);
                contractSummary.ConsumptionAmount += currentContract.ConsumptionAmount;
                discountPresent += currentContract.DiscountPrecent;
                monthlyRateSum += currentContract.MonthlyRate!=null?currentContract.MonthlyRate.Value:0;
                contractSummary.PriceBeforeDiscount += currentContract.PriceBeforeDiscount;
            }
            return new BezekInfoEntity
            {
                Amount = contractSummary.BillingAmount,
                AmountAfterTax = contractSummary.BillingAmountAfterTax,
                Description = "",
                Type = "",
                CallRate = callRateSum/amountOfContracts,
                CallsAmount = contractSummary.CallsAmount,
                CallTime = new TimeSpan(callTimeSum).ToString(),
                DepartmentNumber = defaultValues.DepartmentNumber,
                ClientNumber = defaultValues.ClientNumber,
                ConsumptionAmount = contractSummary.ConsumptionAmount,
                DiscountPrecent = discountPresent/amountOfContracts,
                EndDate = defaultValues.EndDateBilling,
                FreeTimeUsage = new TimeSpan(freeTimeSum).ToString(),
                FreeTimeUsageSupplier = new TimeSpan(freeSupplierTimeSum).ToString(),
                GeneralSummaryRowId = defaultValues.GeneralRowId,
                HebServiceType = "",
                IsMatched = row.Where(x => !x.IsMatched).Count() == 0,
                MonthlyRate = monthlyRateSum/amountOfContracts,
                OriginalClient = defaultValues.OriginalClient,
                OriginalPayer = defaultValues.OriginalPayer,
                PayerNumberBezek = defaultValues.PayerNumberBezek,
                Contract = row.Key,
                PriceBeforeDiscount = contractSummary.PriceBeforeDiscount,
                RowId = 0,
                SecondaryServiceType = "",
                ServiceType = "",
                StartDate = defaultValues.StartDateBilling,
                TimePeriodText = "",
                CustomerId = defaultValues.CustomerId
            };
        }
       
        private async Task<Dictionary<string, BezekInfoEntity[]>> createBezekCodeDictionary(BezekFileInfo[] bezeks, GeneralBillingSummaryEntity summaryEntity)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var data = bezeks.GroupBy(x => x.SubscriptionNumber);
                Dictionary<string, BezekInfoEntity[]> bezekDic = new Dictionary<string, BezekInfoEntity[]>();
                return await Task.Run(() => {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        var groupedRow = data.ElementAt(i);
                        bezekDic.Add(JsonSerializer.Serialize(createSummaryBezekRow(groupedRow), options), createBezekCodeArray(groupedRow.ToArray()));
                    }
                    return bezekDic;
                });
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }
        
    }
}
