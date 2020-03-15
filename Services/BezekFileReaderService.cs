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
    public class BezekFileReaderService : IBezekFileReaderService
    {
        private readonly ManagingDebtsContext context;

        public BezekFileReaderService(ManagingDebtsContext context)
        {
            this.context = context;
        }


        public async Task<bool> GetDataFromExcel(FileEntity fileEntity, bool hasHeader = true)
        {
            var pck = new OfficeOpenXml.ExcelPackage();
                pck.Load(fileEntity.File.OpenReadStream());
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var generalWs = pck.Workbook.Worksheets.First();
                    List<ExcelWorksheet> fullDataWs = new List<ExcelWorksheet>(); ;
                    var Result = InsertExcelRowToGeneralBilling(generalWs.Cells, fileEntity.CustomerId, fileEntity.SupplierId);
                    var generalInsertResult = Result.summary;
                    var invoiceNumber = Result.invoiceNumber;
                    if (generalInsertResult == null)
                        return false;

                    for (int i = 0; i < pck.Workbook.Worksheets.Count; i++)
                    {
                        if (pck.Workbook.Worksheets[i].Name == "חיובים וזיכויים" || pck.Workbook.Worksheets[i].Name == "סכומים מחשבוניות קודמות")
                        {
                            fullDataWs.Add(pck.Workbook.Worksheets[i]);
                        }
                    }
                    if (fullDataWs == null)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    var startRow = hasHeader ? 2 : 1;
                    var result = await disassembleExcel(fullDataWs, hasHeader, generalInsertResult, invoiceNumber);
                    await context.BezekFileInfo.AddRangeAsync(result);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    pck.Dispose();
                    return true;
                }
                catch (Exception e)
                {

                    transaction.Rollback();
                    pck.Dispose();
                    throw e;
                }
            }

        }
        private async Task<BezekFileInfo[]> disassembleExcel(List<ExcelWorksheet> excel, bool hasHeader, GeneralBillingSummary generalInsertResult,string invoiceNumber)
        {
            
            if (generalInsertResult.TotalExtraPayments == 0)
            {
                return await Task.Run(() =>
                {
                    return disassembleCreditDebitWS(excel[0], hasHeader, generalInsertResult,invoiceNumber);
                });
            }
            else
            {
                List<Task<BezekFileInfo[]>> tasks = new List<Task<BezekFileInfo[]>>();
                tasks.Add(Task.Run(() =>
                {
                    return disassembleCreditDebitWS(excel.SingleOrDefault(x => x.Name == "חיובים וזיכויים"), hasHeader, generalInsertResult, invoiceNumber);
                }));
                tasks.Add(Task.Run(() =>
                {
                    return disassemblePreviousPaymentsWS(excel.SingleOrDefault(x => x.Name == "סכומים מחשבוניות קודמות"), hasHeader, generalInsertResult, invoiceNumber, excel.SingleOrDefault(x => x.Name == "חיובים וזיכויים").Dimension.End.Row+1);
                }));
                var result = Task.WhenAll(tasks).Result;
                result[0] = result[0].Concat(result[1]).ToArray();
                return result[0];
            }
           
        }
        private BezekFileInfo[] disassembleCreditDebitWS(ExcelWorksheet excel, bool hasHeader, GeneralBillingSummary generalInsertResult, string invoiceNumber)
        {
            var startRow = hasHeader ? 2 : 1;
            List<BezekFileInfo> bezeks = new List<BezekFileInfo>();
            for (int rowNum = startRow; rowNum <= excel.Dimension.End.Row; rowNum++)
            {

                if (!string.IsNullOrEmpty(excel.Cells[rowNum, 1].Text))
                {
                    bezeks.Add(CreateExcelRowToBezekTable(excel, rowNum, generalInsertResult, invoiceNumber));
                }
                else
                {
                    break;
                }
            }
            return bezeks.ToArray();
        }private BezekFileInfo[] disassemblePreviousPaymentsWS(ExcelWorksheet excel, bool hasHeader, GeneralBillingSummary generalInsertResult, string invoiceNumber, int rowId)
        {
            var startRow = hasHeader ? 2 : 1;
            List<BezekFileInfo> bezeks = new List<BezekFileInfo>();
            for (int rowNum = startRow; rowNum <= excel.Dimension.End.Row; rowNum++)
            {

                if (!string.IsNullOrEmpty(excel.Cells[rowNum, 1].Text))
                {
                    bezeks.Add(CreateExcelRowToBezekTablePreviousPayments(excel, rowNum, generalInsertResult, invoiceNumber, rowId++));
                }
                else
                {
                    break;
                }
            }
            return bezeks.ToArray();
        }
        private BezekFileInfo CreateExcelRowToBezekTable(ExcelWorksheet sheet, int rowNum, GeneralBillingSummary generalSummary, string invoiceNumber)
        {
            try
            {
                BezekFileInfo fileInfo = new BezekFileInfo();
                fileInfo.BillingAmount = string.IsNullOrEmpty(sheet.Cells[rowNum, 9].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 9].Text.Trim());
                fileInfo.BillingDescription = sheet.Cells[rowNum, 8].Text.Trim();
                fileInfo.BillingType = sheet.Cells[rowNum, 7].Text.Trim();
                fileInfo.CallRate = string.IsNullOrEmpty(sheet.Cells[rowNum, 19].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 19].Text.Trim());
                fileInfo.CallsAmount = string.IsNullOrEmpty(sheet.Cells[rowNum, 18].Text.Trim()) ? 0 : int.Parse(sheet.Cells[rowNum, 18].Text.Trim());
                fileInfo.GeneralRowId = generalSummary.RowId;
                fileInfo.CallTime = sheet.Cells[rowNum, 17].Text.Trim();
                fileInfo.DepartmentNumber = sheet.Cells[rowNum, 3].Text.Trim();
                fileInfo.ClientNumber = sheet.Cells[rowNum, 1].Text.Trim();
                fileInfo.ConsumptionAmount = string.IsNullOrEmpty(sheet.Cells[rowNum, 11].Text.Trim()) ? 0 : int.Parse(sheet.Cells[rowNum, 11].Text.Trim());
                fileInfo.TaxRate = string.IsNullOrEmpty(sheet.Cells[rowNum, 10].Text.Trim()) ? 0 : int.Parse(sheet.Cells[rowNum, 10].Text.Trim());
                fileInfo.CustomerId = generalSummary.CustomerId;
                fileInfo.DiscountPrecent = string.IsNullOrEmpty(sheet.Cells[rowNum, 16].Text.Trim()) ? 0.0 : double.Parse(sheet.Cells[rowNum, 16].Text.Trim());
                fileInfo.EndDateBilling = string.IsNullOrEmpty(sheet.Cells[rowNum, 6].Text.Trim()) ? generalSummary.BillToDate : DateTime.ParseExact(sheet.Cells[rowNum, 6].Text.Trim(), "dd/MM/yyyy", null);
                fileInfo.FreeTimeUsage = sheet.Cells[rowNum, 20].Text.Trim();
                fileInfo.FreeTimeUsageSupplier = sheet.Cells[rowNum, 20].Text.Trim();
                fileInfo.HebServiceType = sheet.Cells[rowNum, 25].Text.Trim();
                fileInfo.MonthlyRate = string.IsNullOrEmpty(sheet.Cells[rowNum, 12].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 12].Text.Trim());
                fileInfo.OriginalClient = sheet.Cells[rowNum, 14].Text.Trim();
                fileInfo.OriginalPayer = string.IsNullOrEmpty(sheet.Cells[rowNum, 15].Text.Trim()) ? 0 : int.Parse(sheet.Cells[rowNum, 15].Text.Trim());
                fileInfo.PayerNumberBezek = Convert.ToInt32(sheet.Cells[rowNum, 2].Text.Trim());
                fileInfo.PriceBeforeDiscount = string.IsNullOrEmpty(sheet.Cells[rowNum, 13].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 13].Text.Trim());
                fileInfo.SecondaryServiceType = sheet.Cells[rowNum, 24].Text.Trim();
                fileInfo.ServiceType = sheet.Cells[rowNum, 23].Text.Trim();
                fileInfo.StartDateBilling = string.IsNullOrEmpty(sheet.Cells[rowNum, 5].Text.Trim()) ? generalSummary.BillFromDate : DateTime.ParseExact(sheet.Cells[rowNum, 5].Text.Trim(), "dd/MM/yyyy", null);
                fileInfo.SubscriptionNumber = Convert.ToInt32(sheet.Cells[rowNum, 4].Text.Trim().Replace("-", ""));
                fileInfo.TimePeriodText = sheet.Cells[rowNum, 21].Text.Trim();
                fileInfo.IsMatched = false;
                fileInfo.RowId = rowNum;
                fileInfo.InvoiceNumber = invoiceNumber;
                fileInfo.BillingAmountAfterTax =Math.Round(fileInfo.BillingAmount * ((fileInfo.TaxRate != 0 ? Convert.ToDecimal(fileInfo.TaxRate) / 100 : 0) + 1),2);
                return fileInfo;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        private BezekFileInfo CreateExcelRowToBezekTablePreviousPayments(ExcelWorksheet sheet, int rowNum, GeneralBillingSummary generalSummary, string invoiceNumber, int rowId)
        {
            try
            {
                BezekFileInfo fileInfo = new BezekFileInfo();
                fileInfo.BillingAmountAfterTax = string.IsNullOrEmpty(sheet.Cells[rowNum, 7].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 7].Text.Trim());
                fileInfo.BillingAmount = string.IsNullOrEmpty(sheet.Cells[rowNum, 5].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 5].Text.Trim());
                fileInfo.BillingDescription = sheet.Cells[rowNum, 10].Text.Trim();
                fileInfo.BillingType = sheet.Cells[rowNum, 4].Text.Trim();
                fileInfo.CallRate = 0;
                fileInfo.CallsAmount = 0;
                fileInfo.GeneralRowId = generalSummary.RowId;
                fileInfo.CallTime = "";
                fileInfo.DepartmentNumber = "";
                fileInfo.ClientNumber = sheet.Cells[rowNum, 1].Text.Trim();
                fileInfo.ConsumptionAmount = 0;
                fileInfo.CustomerId = generalSummary.CustomerId;
                fileInfo.DiscountPrecent = 0;
                fileInfo.FreeTimeUsage = "";
                fileInfo.FreeTimeUsageSupplier ="";
                fileInfo.HebServiceType = "";
                fileInfo.OriginalClient = "";
                fileInfo.OriginalPayer = 0;
                int paymentsLeft = string.IsNullOrEmpty(sheet.Cells[rowNum, 15].Text.Trim()) ? 0 : int.Parse(sheet.Cells[rowNum, 15].Text.Trim());
                decimal paymentsSoFar = string.IsNullOrEmpty(sheet.Cells[rowNum, 13].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 13].Text.Trim());
                int totalPayments = Convert.ToInt32(paymentsSoFar / fileInfo.BillingAmountAfterTax) +paymentsLeft;
                fileInfo.MonthlyRate = fileInfo.BillingAmountAfterTax;
                fileInfo.PayerNumberBezek = Convert.ToInt32(sheet.Cells[rowNum, 2].Text.Trim());
                fileInfo.PriceBeforeDiscount = 0;
                fileInfo.SecondaryServiceType = "";
                fileInfo.ServiceType = "";
                fileInfo.StartDateBilling = string.IsNullOrEmpty(sheet.Cells[rowNum, 9].Text.Trim()) ? generalSummary.BillFromDate : DateTime.ParseExact(sheet.Cells[rowNum, 9].Text.Trim(), "dd/MM/yyyy", null);
                fileInfo.EndDateBilling = totalPayments != 0 ? fileInfo.StartDateBilling.Value.AddMonths(totalPayments) : fileInfo.StartDateBilling.Value;
                fileInfo.SubscriptionNumber = Convert.ToInt32(sheet.Cells[rowNum, 3].Text.Trim().Replace("-", ""));
                fileInfo.TimePeriodText = "";
                fileInfo.IsMatched = false;
                fileInfo.RowId = rowId;
                fileInfo.InvoiceNumber = invoiceNumber;
                decimal taxAmount = string.IsNullOrEmpty(sheet.Cells[rowNum, 6].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 6].Text.Trim());
                fileInfo.TaxRate = taxAmount !=0 &&fileInfo.BillingAmount != 0 ? Convert.ToInt32((((taxAmount+fileInfo.BillingAmount)/ fileInfo.BillingAmount) -1 ) * 100) : 0;
                return fileInfo;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        private (GeneralBillingSummary summary,string invoiceNumber) InsertExcelRowToGeneralBilling(ExcelRange row, int customerId, string supplierId)
        {
            try
            {
                var maxEntity = context.GeneralBillingSummary;
                int maxId = maxEntity != null && maxEntity.Count() > 0 ? maxEntity.Max(x => x.RowId) + 1 : 1;
                GeneralBillingSummary generalBillingSummary = new GeneralBillingSummary();
                generalBillingSummary.CustomerId = customerId;
                generalBillingSummary.BillFromDate = DateTime.ParseExact(row[2, 7].Text, "dd/MM/yyyy", null);
                generalBillingSummary.BillToDate = DateTime.ParseExact(row[2, 8].Text, "dd/MM/yyyy", null);
                generalBillingSummary.SupplierClientNumber = Convert.ToInt32(row[2, 2].Text);
                generalBillingSummary.SupplierId = supplierId;
                generalBillingSummary.SupplierPayerId = Convert.ToInt32(row[2, 1].Text);
                generalBillingSummary.TotalInvoice = Convert.ToDecimal(row[2, 20].Text);
                generalBillingSummary.TotalInvoiceBeforeTax = Convert.ToDecimal(row[2, 15].Text);
                generalBillingSummary.TotalChangableBilling = Convert.ToDecimal(row[2, 10].Text);
                generalBillingSummary.TotalCredit = Convert.ToDecimal(row[2, 13].Text);
                generalBillingSummary.TotalDebit = Convert.ToDecimal(row[2, 24].Text);
                generalBillingSummary.TotalFixedBilling = Convert.ToDecimal(row[2, 9].Text);
                generalBillingSummary.TotalOneTimeBilling = Convert.ToDecimal(row[2, 11].Text);
                generalBillingSummary.TotalExtraPayments = Convert.ToDecimal(row[2, 21].Text)+ Convert.ToDecimal(row[2, 22].Text);
                generalBillingSummary.DateOfValue = DateTime.ParseExact(row[2, 4].Text, "dd/MM/yyyy", null);
                generalBillingSummary.RowId = maxId;
                context.GeneralBillingSummary.Add(generalBillingSummary);
                return (generalBillingSummary, row[2, 6].Text);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
