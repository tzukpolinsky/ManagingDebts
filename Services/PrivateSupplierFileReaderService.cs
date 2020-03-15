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
    public class PrivateSupplierFileReaderService : IPrivateSupplierFileReaderService
    {
        private readonly ManagingDebtsContext context;

        public PrivateSupplierFileReaderService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public async Task<bool> GetDataFromExcel(FileEntity fileEntity, bool hasHeader = true)
        {
            var pck = new ExcelPackage();
            pck.Load(fileEntity.File.OpenReadStream());
            if (pck.Workbook.Worksheets.Count >1)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var generalWs = pck.Workbook.Worksheets.SingleOrDefault(x => x.Name == "סיכום חשבון");
                        if (generalWs == null)
                            generalWs = pck.Workbook.Worksheets.First();
                        
                        var generalBillingSummary = createGeneralBillingDefaults(fileEntity,generalWs);
                        ExcelWorksheet fullDataWs = pck.Workbook.Worksheets.SingleOrDefault(x => x.Name == "פירוט");
                        if (fullDataWs == null)
                            fullDataWs = pck.Workbook.Worksheets[1];
                        var result = await disassembleExcel(fullDataWs, hasHeader, generalBillingSummary);
                        if (result.GroupBy(x => x.Invoice).Count() == 1)
                        {
                            generalBillingSummary.TotalInvoice = result.Sum(x => x.AmountAfterTax);
                            generalBillingSummary.TotalInvoiceBeforeTax =  result.Sum(x => x.Amount);
                        }
                        await context.PrivateSupplierFileInfo.AddRangeAsync(result);
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
            pck.Dispose();
            return false;
        }
        private async Task<PrivateSupplierFileInfo[]> disassembleExcel(ExcelWorksheet excel, bool hasHeader,GeneralBillingSummary generalInsertResult)
        {
                return await Task.Run(() =>
                {
                    var startRow = hasHeader ? 2 : 1;
                    List<PrivateSupplierFileInfo> privateSuppliers = new List<PrivateSupplierFileInfo>();
                    for (int rowNum = startRow; rowNum <= excel.Dimension.End.Row; rowNum++)
                    {

                        if (!string.IsNullOrEmpty(excel.Cells[rowNum, 1].Text))
                        {
                            var result = CreateExcelRowToPrivateTable(excel, rowNum,ref generalInsertResult);
                            if (result !=null)
                            {
                                privateSuppliers.Add(result);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    return privateSuppliers.ToArray(); ;
                });
            
        }
        private PrivateSupplierFileInfo CreateExcelRowToPrivateTable(ExcelWorksheet sheet, int rowNum,ref GeneralBillingSummary generalSummary)
        {
            try
            {
                PrivateSupplierFileInfo fileInfo = new PrivateSupplierFileInfo();
               
                int contract = 0;
                if(int.TryParse(sheet.Cells[rowNum, 2].Text.Trim(), out contract))
                {
                    fileInfo.Contract =contract;
                    int invoice = 0;
                    if (int.TryParse(sheet.Cells[rowNum, 1].Text.Trim(),out invoice))
                    {
                        DateTime dateOfValue =new DateTime(1,1,1);
                        if (DateTime.TryParseExact(sheet.Cells[rowNum, 5].Text.Trim(), "dd/MM/yyyy", null,System.Globalization.DateTimeStyles.AssumeLocal,out dateOfValue))
                        {
                            fileInfo.Amount = string.IsNullOrEmpty(sheet.Cells[rowNum, 4].Text.Trim()) ? 0 : decimal.Parse(sheet.Cells[rowNum, 4].Text.Trim());
                            fileInfo.TaxRate = string.IsNullOrEmpty(sheet.Cells[rowNum, 6].Text.Trim().Replace("%","")) ? 0 : int.Parse(sheet.Cells[rowNum, 6].Text.Trim().Replace("%", ""));
                            fileInfo.AmountAfterTax = fileInfo.Amount * (fileInfo.TaxRate != 0 ? (Convert.ToDecimal(fileInfo.TaxRate) / 100) : 0 + 1);
                            generalSummary.TotalCredit += fileInfo.AmountAfterTax < 0 ? -1 * fileInfo.AmountAfterTax : 0;
                            generalSummary.TotalDebit += fileInfo.AmountAfterTax < 0 ? fileInfo.AmountAfterTax : 0;
                            fileInfo.Description = sheet.Cells[rowNum, 3].Text.Trim();
                            fileInfo.GeneralRowId = generalSummary.RowId;
                            fileInfo.CustomerId = generalSummary.CustomerId;
                            fileInfo.DateOfValue = dateOfValue;
                            fileInfo.RowId = rowNum;
                            fileInfo.Invoice = invoice;
                            return fileInfo;
                        }                       
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        private GeneralBillingSummary createGeneralBillingDefaults(FileEntity file, ExcelWorksheet sheet)
        {
            var maxEntity = context.GeneralBillingSummary;
            int maxId = maxEntity != null && maxEntity.Count() > 0 ? maxEntity.Max(x => x.RowId) + 1 : 1;
            DateTime now = DateTime.Now;
            DateTime fromDate = now.AddMonths(-1);
            GeneralBillingSummary summary = new GeneralBillingSummary()
            {
                CustomerId = file.CustomerId,
                SupplierId = file.SupplierId,
                RowId = maxId,
                SupplierPayerId = 0,
                SupplierClientNumber = 0,
                Sent = false,
                TotalInvoice = 0,
                TotalInvoiceBeforeTax = 0,
            };
            summary.BillFromDate = string.IsNullOrEmpty(sheet.Cells[2, 1].Text.Trim()) ? new DateTime(fromDate.Year, fromDate.Month, DateTime.DaysInMonth(fromDate.Year, fromDate.Month)) : DateTime.ParseExact(sheet.Cells[2, 1].Text.Trim(), "dd/MM/yyyy", null);
            summary.BillToDate = string.IsNullOrEmpty(sheet.Cells[2, 2].Text.Trim()) ? new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)) : DateTime.ParseExact(sheet.Cells[2, 2].Text.Trim(), "dd/MM/yyyy", null);
            int totalFixedBilling = 0;
            summary.TotalFixedBilling = int.TryParse(sheet.Cells[2, 3].Text.Trim(), out totalFixedBilling) ? totalFixedBilling : 0;
            int totalChangableBilling = 0;
            summary.TotalChangableBilling = int.TryParse(sheet.Cells[2, 4].Text.Trim(), out totalChangableBilling) ? totalChangableBilling : 0;
            int totalOneTimeBilling = 0;
            summary.TotalOneTimeBilling = int.TryParse(sheet.Cells[2, 5].Text.Trim(), out totalOneTimeBilling) ? totalOneTimeBilling : 0;
            int totalExtraPayments = 0;
            summary.TotalExtraPayments = int.TryParse(sheet.Cells[2, 6].Text.Trim(), out totalExtraPayments) ? totalExtraPayments : 0;
            return summary;
        }
    }
}
