using Entities;
using ManagingDebts;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ElectricityFileReaderService: IElectricityFileReaderService
    {
        private readonly ManagingDebtsContext context;
        public ElectricityFileReaderService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public async Task<bool> GetDataFromExcel(FileEntity fileEntity, bool hasHeader = true)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    
                    GeneralBillingSummary billingSummary = createGeneralBillingDefaults(fileEntity);
                    var result = await Task.Run(() => {
                        using (var file = new StreamReader(fileEntity.File.OpenReadStream(),Encoding.GetEncoding("Windows-1255")))
                        {
                       
                            string line = "";
                            int counter = 1;
                            List<ElectricityFileInfo> electricities = new List<ElectricityFileInfo>();
                            while ((line = file.ReadLine()) != null)
                            {
                                electricities.Add(createElectricityFileInfoRow(line, counter++, ref billingSummary));
                            }
                            billingSummary.BillFromDate = electricities.Min(x => x.BillCreatingDate);
                            billingSummary.BillToDate = electricities.Max(x => x.BillCreatingDate);
                            billingSummary.TotalChangableBilling = billingSummary.TotalDebit;
                            context.ElectricityFileInfo.AddRange(electricities);
                            return true;
                        }
                    });
                    if (result)
                    {
                        context.GeneralBillingSummary.Add(billingSummary);
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
        private GeneralBillingSummary createGeneralBillingDefaults(FileEntity file)
        {
            var maxEntity = context.GeneralBillingSummary;
            int  maxId = maxEntity != null && maxEntity.Count() > 0? maxEntity.Max(x => x.RowId) + 1:1;
            return new GeneralBillingSummary
            {
                CustomerId = file.CustomerId,
                SupplierId = file.SupplierId,
                RowId = maxId,
                SupplierPayerId = 0,
                SupplierClientNumber = 0,
                Sent = false,
                TotalFixedBilling = 0,
                TotalExtraPayments =0,
                TotalInvoice = 0,
                TotalInvoiceBeforeTax = 0,
                TotalOneTimeBilling = 0,
            };
        }
        private ElectricityFileInfo createElectricityFileInfoRow(string line,int rowId,ref GeneralBillingSummary summary)
        {
            string[] values = line.Split(';');
            ElectricityFileInfo info = new ElectricityFileInfo();
            info.GeneralRowId = summary.RowId;
            info.RowId = rowId;
            info.CustomerId = summary.CustomerId;
            info.BillCreatingDate = DateTime.ParseExact(values[6], "yyMMdd", null);
            info.NumberOfCreditDays = Convert.ToInt32(values[7].Substring(1));
            info.Amount = Convert.ToDecimal(values[8]) / 100;
            info.PaymentDate = DateTime.ParseExact(values[9], "yyMMdd", null);
            info.ConsumerAddress = values[11];
            info.ConsumerName = values[10];
            info.BankCode = Convert.ToInt32(values[13]);
            info.BankBranch = Convert.ToInt32(values[14]);
            info.BankAccount = Convert.ToInt32(values[15]);
            info.BankAccountType = Convert.ToInt32(values[16]);
            info.MonthOfLastInvoice = DateTime.ParseExact(values[17], "MMyy",null).Month;
            info.YearOfLastInvoice = DateTime.ParseExact(values[17], "MMyy",null).Year;
            info.ConsumerNumber = Convert.ToInt32(values[18]);
            info.Contract = Convert.ToInt32(values[19]);
            info.Invoice = Convert.ToInt64(values[20]);
            summary.DateOfValue = summary.DateOfValue.Year != 1 ? summary.DateOfValue : new DateTime(info.PaymentDate.Year, info.PaymentDate.Month, DateTime.DaysInMonth(info.PaymentDate.Year, info.PaymentDate.Month));
            summary.TotalCredit += info.Amount < 0 ? -1*info.Amount : 0;
            summary.TotalDebit += info.Amount >= 0 ? info.Amount : 0;
            return info;
        }
    }
}
