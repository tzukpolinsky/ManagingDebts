using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GeneralBillingSummaryService : IGeneralBillingSummaryService
    {
        private readonly ManagingDebtsContext context;

        public GeneralBillingSummaryService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public GeneralBillingSummaryEntity[] GetSummaryLines(GeneralBillingSummaryEntity generalBillingSummaryEntity)
        {
            var generalBillingSummaries = context.GeneralBillingSummary.Where(x => x.CustomerId == generalBillingSummaryEntity.CustomerId && x.SupplierId == generalBillingSummaryEntity.SupplierId)
                .Select(data => new GeneralBillingSummaryEntity {
                    BillingFrom = data.BillFromDate,
                    BillingTo = data.BillToDate,
                    TotalDebit = data.TotalDebit,
                    TotalBilling = data.TotalInvoice,
                    TotalBillingBeforeTax = data.TotalInvoiceBeforeTax,
                    TotalChangable = data.TotalChangableBilling,
                    TotalCredit = data.TotalCredit,
                    TotalFixed = data.TotalFixedBilling,
                    TotalOneTime = data.TotalOneTimeBilling,
                    CustomerId = data.CustomerId,
                    SupplierId = data.SupplierId,
                    RowId = data.RowId,
                    IsSent = data.Sent,
                    DateOfValue = data.DateOfValue,
                    TotalExtraPayments = data.TotalExtraPayments,
                    SupplierClientNumber = data.SupplierClientNumber
                }).AsNoTracking();
            return generalBillingSummaries != null ? generalBillingSummaries.ToArray() : null;
        }
        public bool Delete(GeneralBillingSummaryEntity generalBillingSummaryEntity)
        {
            try
            {
                switch (Convert.ToInt32(generalBillingSummaryEntity.SupplierId))
                {
                    case (int) Enums.Suppliers.Bezek:
                        context.BezekFileInfo.RemoveRange(context.BezekFileInfo.Where(x => x.CustomerId == generalBillingSummaryEntity.CustomerId && x.GeneralRowId == generalBillingSummaryEntity.RowId));
                        break;
                    case (int)Enums.Suppliers.Electricity:
                        context.ElectricityFileInfo.RemoveRange(context.ElectricityFileInfo.Where(x => x.CustomerId == generalBillingSummaryEntity.CustomerId && x.GeneralRowId == generalBillingSummaryEntity.RowId));
                        break;
                    default:
                        context.PrivateSupplierFileInfo.RemoveRange(context.PrivateSupplierFileInfo.Where(x => x.CustomerId == generalBillingSummaryEntity.CustomerId && x.GeneralRowId == generalBillingSummaryEntity.RowId &&x.SupplierId == generalBillingSummaryEntity.SupplierId));
                        break;
                }
                context.GeneralBillingSummary.Remove(context.GeneralBillingSummary.Where(x => x.CustomerId == generalBillingSummaryEntity.CustomerId && x.RowId == generalBillingSummaryEntity.RowId).FirstOrDefault());
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
