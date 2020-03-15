using Entities;
using ManagingDebts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PkudatYomanWebService;
namespace Services
{
    public class SliService : ISliService
    {
        private readonly ManagingDebtsContext context;

        public SliService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public PkudatYomanShura[] DisplayPkudatYoman(GeneralBillingSummaryEntity summary, DateTime dateOfRegistration)
        {
            try
            {
                var dbSummary = context.GeneralBillingSummary.SingleOrDefault(x => x.RowId == summary.RowId && x.CustomerId == summary.CustomerId && x.SupplierId == summary.SupplierId);
                if (dbSummary!= null)
                {

                
                var rows = GeneratePkudatYoman(dbSummary, dateOfRegistration).shuras;
                foreach (var row in rows)
                {
                    row.Teur = new string(row.Teur.ToCharArray().Reverse().ToArray());
                    row.Teur = row.Teur.Replace("datetime", summary.DateOfValue.ToString("MM/yy"));
                    row.Teur = row.Teur.Replace("clientnumber", dbSummary.SupplierClientNumber.ToString());
                }
                return rows;
                } else
                {
                    return null;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        private (PkudatYomanShura[] shuras,bool isBalanced) GeneratePkudatYoman(GeneralBillingSummary dbSummary, DateTime dateOfRegistration,SupplierEntity supplier =null)
        {
            string dateOfRegistrationString = dateOfRegistration.ToString("yyyyMMdd");
            string DateOfValueString = dbSummary.DateOfValue.ToString("yyyyMMdd");
            if (dbSummary !=null)
            {          
                supplier = supplier == null ? new SuppliersService(context).GetSuppliersByCustomer(new CustomerEntity { Id = dbSummary.CustomerId }).FirstOrDefault(x => x.Id == dbSummary.SupplierId && x.IsEnable):supplier;
                var budgetsContracts = new BudgetContractService(context).GetRelationshipBySupplier(supplier);
                var contracts = new ContractsService(context).GetBySupplier(supplier).Result;
                var banks = new BankAccountsService(context).GetBySupplier(supplier);
                switch (Convert.ToInt32(dbSummary.SupplierId))
                {
                    case (int)Enums.Suppliers.Bezek:
                        return createPkudaForBezek(dbSummary, dateOfRegistrationString, supplier, DateOfValueString, budgetsContracts,contracts,banks).Result;
                    case (int)Enums.Suppliers.Electricity:
                        return createPkudaForElectricity(dbSummary, dateOfRegistrationString, supplier, DateOfValueString, budgetsContracts, contracts, banks).Result;
                    default:
                        return createPkudaForPrivateSupplier(dbSummary, dateOfRegistrationString, supplier, DateOfValueString, budgetsContracts, contracts, banks).Result;

                }
            }
            return (null, false);
        }
        public (bool isSuccess, string msg) CreatePkudatYoman(GeneralBillingSummaryEntity summary, DateTime dateOfRegistration, UserEntity user)
        {
            try
            { 
                var supplier = new SuppliersService(context).GetSuppliersByCustomer(new CustomerEntity { Id = summary.CustomerId }).FirstOrDefault(x => x.Id == summary.SupplierId && x.IsEnable);
                var dbSummary = context.GeneralBillingSummary.SingleOrDefault(x => x.RowId == summary.RowId &&x.CustomerId ==summary.CustomerId &&x.SupplierId == summary.SupplierId);
                var result = GeneratePkudatYoman(dbSummary, dateOfRegistration);
                if (result.shuras == null || !result.isBalanced)
                {
                    return (false, "הפקודה אינה מאוזנת");
                }
                return sendPkuda(result.shuras, supplier, dbSummary, user, dateOfRegistration);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private (bool isSuccess, string msg) sendPkuda(PkudatYomanShura[] yomanShuras,SupplierEntity supplier,GeneralBillingSummary summary,UserEntity user, DateTime dateOfRegistration)
        {
            SystemHeader systemHeader = getSystemHeader(user);
            CreatePkudatYomanRequest createPkudatYomanRequest = getYomanRequest(user, supplier, dateOfRegistration.ToString("yyyyMMdd"), summary.DateOfValue.ToString("yyyyMMdd"));
            createPkudatYomanRequest.Rows = yomanShuras;
            foreach (var row in createPkudatYomanRequest.Rows)
            {
                row.Teur = row.Teur.Replace("emitetad", summary.DateOfValue.ToString("MM/yy"));
                row.Teur = row.Teur.Replace("rebmuntneilc", summary.SupplierClientNumber.ToString());
            }
            return (true, "פעולת היומן שוגרה בהצלחה לפיננסים"); //delete in case of real sending
            //var msg = new PkudatYomanWSClient().createPkudatYoman(systemHeader, createPkudatYomanRequest);
            //if (msg.Msg.RcType == 1)
            //{
            //    var dbSupplier = context.Suppliers.FirstOrDefault(x => x.SupplierCustomerId == supplier.CustomerId && x.SupplierId == supplier.Id);
            //    dbSupplier.SupplierPkudatYomanNumber += 1;
            //    summary.Sent = true;
            //    context.SaveChanges();
            //    return (true, "פעולת היומן שוגרה בהצלחה לפיננסים");
            //}
            //else
            //{
            //    return (false, msg.Msg.RcMessage);
            //}
        }
        private CreatePkudatYomanRequest getYomanRequest(UserEntity user, SupplierEntity supplier, string dateOfRegistration, string DateOfValueString)
        {
            return new CreatePkudatYomanRequest
            {
                Gorem = 1, // need to be create in finance
                Makor = 72,
                Status = 10,
                Email = user.Email,
                TaarichErech = DateOfValueString,
                TaarichRishum = dateOfRegistration,
                MisparPkuda = supplier.PkudatYomanNumber,
            };
        }
        private SystemHeader getSystemHeader(UserEntity user)
        {
            return new SystemHeader
            {
                Customer = user.CustomerId,
                Sender = 33,
                Recipient = 33,
                Token = "0",
                TranID = "0",
                Version = "0",
                UserId = user.Id,
                UserPass = user.Password
            };
        }
        private async Task<(PkudatYomanShura[] shuras, bool isBalanced)> createPkudaForBezek(GeneralBillingSummary summary, string dateOfRegistration, SupplierEntity supplier, string DateOfValueString,
            BudgetContractEntity[] budgetsContracts,ContractEntity[] contracts,BankAccountEntity[] banks)
        {
            var dbData =await context.BezekFileInfo.Where(x => x.GeneralRowId == summary.RowId && x.CustomerId == summary.CustomerId && x.IsMatched).ToArrayAsync();
            var invoiceData = dbData.GroupBy(x => x.InvoiceNumber);
            int rowAmount = 0;
            Dictionary<long, Dictionary<(long, int), PkudatYomanShura>> invoices = new Dictionary<long, Dictionary<(long, int), PkudatYomanShura>>();
            return await Task.Run(() =>
            {
                foreach (var invoice in invoiceData)
                {
                    var data = invoice.GroupBy(x => x.SubscriptionNumber);
                    Dictionary<(long,int), PkudatYomanShura> pkudatYomanShuras = new Dictionary<(long, int), PkudatYomanShura>();
                    Dictionary<(long, int), PkudatYomanShura> pkudatYomanShurasBanks = new Dictionary<(long, int), PkudatYomanShura>();
                    pkudatYomanShuras.Add((supplier.SupplierNumberInFinance,2), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key));
                    foreach (var row in data)
                    {
                        var contract = contracts.FirstOrDefault(x => x.Id == row.Key);
                        if (contract == null)
                        {
                            break;
                        }
                        var budgets = getContractBudgets(budgetsContracts, row.Key);
                        for (int j = 0; j < budgets.Length; j++)
                        {
                            double amount = row.Sum(x => Convert.ToDouble(x.BillingAmountAfterTax)) * budgets[j].Precent / 100;
                            PkudatYomanShura pkudatYomanShura = new PkudatYomanShura();
                            if (pkudatYomanShuras.TryGetValue((budgets[j].BudgetId,1), out pkudatYomanShura))
                            {
                                pkudatYomanShura.Schum += amount;
                            }
                            else
                            {
                                pkudatYomanShura = createPkudatYomanBudgetWithOutAmount(summary, supplier, dateOfRegistration, DateOfValueString, budgets[j].BudgetId, invoice.Key);
                                //pkudatYomanShura.SugMaam = 1;// for business client like water corporations null if it's a regular client
                                pkudatYomanShura.Schum = amount;
                                pkudatYomanShuras.Add((budgets[j].BudgetId, 1), pkudatYomanShura);
                            }
                            if (supplier.WithBanks)
                            {                      
                                pkudatYomanShura = new PkudatYomanShura();
                                if (pkudatYomanShurasBanks.TryGetValue((contract.BankAccountInFinance,2), out pkudatYomanShura))
                                {
                                    pkudatYomanShura.Schum += amount;
                                }
                                else
                                {
                                    pkudatYomanShura = createPkudatYomanBankWithOutAmount(summary, supplier, dateOfRegistration, DateOfValueString, contract.BankAccountInFinance, invoice.Key);
                                    pkudatYomanShura.Schum = amount;
                                    pkudatYomanShurasBanks.Add((contract.BankAccountInFinance, 2), pkudatYomanShura);
                                }
                            }
                        }
                        foreach (var item in row)
                        {
                            item.JournalEntryNumber = supplier.PkudatYomanNumber;
                        }
                    }
                    if (supplier.WithBanks)
                    {
                        pkudatYomanShurasBanks.Add((supplier.SupplierNumberInFinance,1), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key.ToString()));
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].ZchutChovaInd = 1;
                        //if only one bank we add counter account else it's zero
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].MisparCheshbonNegdi = pkudatYomanShurasBanks.Count == 2 ? pkudatYomanShurasBanks.ElementAt(0).Value.MisparCheshbon : 0;
                        pkudatYomanShuras = pkudatYomanShuras.Concat(pkudatYomanShurasBanks).ToDictionary(x => x.Key, x => x.Value);
                    }
                    invoices.Add(Convert.ToInt64(invoice.Key), pkudatYomanShuras);
                    rowAmount += pkudatYomanShuras.Values.Count;
                }
               
                PkudatYomanShura[] shuras = new PkudatYomanShura[rowAmount];
                double budgetSum = 0;
                foreach (var invoice in invoices)
                {
                    foreach ((PkudatYomanShura row, int i) in invoice.Value.Values.Select((value, i) => (value, i)))
                    {
                        budgetSum += row.ZchutChovaInd == 2 ? -1 * row.Schum.Value : row.ZchutChovaInd == 1 ? row.Schum.Value : 0;
                        row.Schum = Math.Round(Convert.ToDouble(row.Schum), 2);
                        shuras[i] = row;
                    }
                }
                return (shuras, budgetSum> -0.01 && budgetSum < 0.01);
            });
        } 
        private async Task<(PkudatYomanShura[] shuras, bool isBalanced)> createPkudaForElectricity(GeneralBillingSummary summary, string dateOfRegistration, SupplierEntity supplier, string DateOfValueString,
            BudgetContractEntity[] budgetsContracts, ContractEntity[] contracts, BankAccountEntity[] banks)
        {
            var dbData =await context.ElectricityFileInfo.Where(x => x.GeneralRowId == summary.RowId && x.CustomerId == summary.CustomerId && x.IsMatched).ToArrayAsync();
            var invoiceData = dbData.GroupBy(x => x.Invoice);
            Dictionary<long, Dictionary<(long, int), PkudatYomanShura>> invoices = new Dictionary<long, Dictionary<(long, int), PkudatYomanShura>>();
            return await Task.Run(() =>
            {
                foreach (var invoice in invoiceData)
                {
                    var data = invoice.GroupBy(x => x.Contract);
                    Dictionary<(long,int), PkudatYomanShura> pkudatYomanShuras = new Dictionary<(long, int), PkudatYomanShura>();
                    Dictionary<(long, int), PkudatYomanShura> pkudatYomanShurasBanks = new Dictionary<(long, int), PkudatYomanShura>();
                    pkudatYomanShuras.Add((supplier.SupplierNumberInFinance,2), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key.ToString()));
                    foreach (var row in data)
                    {
                        var contract = contracts.FirstOrDefault(x => x.Id == row.Key);
                        if (contract == null)
                        {
                            break;
                        }
                        var budgets = getContractBudgets(budgetsContracts, row.Key);
                        for (int j = 0; j < budgets.Length; j++)
                        {
                            double amount = row.Sum(x => Convert.ToDouble(x.Amount)) * budgets[j].Precent / 100;
                            PkudatYomanShura pkudatYomanShura = new PkudatYomanShura();
                            if (pkudatYomanShuras.TryGetValue((budgets[j].BudgetId,1), out pkudatYomanShura))
                            {
                                pkudatYomanShura.Schum += amount;
                            }
                            else
                            {
                                pkudatYomanShura = createPkudatYomanBudgetWithOutAmount(summary, supplier, dateOfRegistration, row.First().PaymentDate.ToString("yyyyMMdd"), budgets[j].BudgetId, invoice.Key.ToString());
                                //pkudatYomanShura.SugMaam = 1;// for business client like water corporations null if it's a regular client
                                pkudatYomanShura.Schum = amount;
                                pkudatYomanShuras.Add((budgets[j].BudgetId, 1), pkudatYomanShura);
                            }
                            if (supplier.WithBanks)
                            {                      
                                pkudatYomanShura = new PkudatYomanShura();
                                if (pkudatYomanShurasBanks.TryGetValue((contract.BankAccountInFinance,2), out pkudatYomanShura))
                                {
                                    pkudatYomanShura.Schum += amount;
                                }
                                else
                                {
                                    pkudatYomanShura = createPkudatYomanBankWithOutAmount(summary, supplier, dateOfRegistration, DateOfValueString, contract.BankAccountInFinance, invoice.Key.ToString());
                                    pkudatYomanShura.Schum = amount;
                                    pkudatYomanShurasBanks.Add((contract.BankAccountInFinance, 2), pkudatYomanShura);
                                }
                            }
                        }
                        foreach (var item in row)
                        {
                            item.JournalEntryNumber = supplier.PkudatYomanNumber;
                        }
                    }
                    if (supplier.WithBanks)
                    {
                        pkudatYomanShurasBanks.Add((supplier.SupplierNumberInFinance,1), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key.ToString()));
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].ZchutChovaInd = 1;
                        //if only one bank we add counter account else it's zero
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].MisparCheshbonNegdi = pkudatYomanShurasBanks.Count == 2 ? pkudatYomanShurasBanks.ElementAt(0).Value.MisparCheshbon : 0;
                        pkudatYomanShuras = pkudatYomanShuras.Concat(pkudatYomanShurasBanks).ToDictionary(x => x.Key, x => x.Value);
                    }
                    invoices.Add(Convert.ToInt64(invoice.Key), pkudatYomanShuras);
                }

                List<PkudatYomanShura> shuras = new List<PkudatYomanShura>();
                double budgetSum = 0;
                foreach (var invoice in invoices)
                {
                    foreach ((PkudatYomanShura row, int i) in invoice.Value.Values.Select((value, i) => (value, i)))
                    {
                        budgetSum += row.ZchutChovaInd == 2 ? -1 * row.Schum.Value : row.ZchutChovaInd == 1 ? row.Schum.Value : 0;
                        row.Schum = Math.Round(Convert.ToDouble(row.Schum), 2);
                        shuras.Add(row);
                    }
                }
                return (shuras.ToArray(), budgetSum > -0.01 && budgetSum < 0.01);
            });
        }
        private async Task<(PkudatYomanShura[] shuras, bool isBalanced)> createPkudaForPrivateSupplier(GeneralBillingSummary summary, string dateOfRegistration, SupplierEntity supplier, string DateOfValueString,
            BudgetContractEntity[] budgetsContracts, ContractEntity[] contracts, BankAccountEntity[] banks)
        {
            var dbData =await context.PrivateSupplierFileInfo.Where(x => x.GeneralRowId == summary.RowId && x.CustomerId == summary.CustomerId && x.IsMatched && x.SupplierId == supplier.Id).ToArrayAsync();
            var invoiceData = dbData.GroupBy(x => x.Invoice);
            Dictionary<long, Dictionary<(long, int), PkudatYomanShura>> invoices = new Dictionary<long, Dictionary<(long, int), PkudatYomanShura>>();
            return await Task.Run(() =>
            {
                foreach (var invoice in invoiceData)
                {
                    var data = invoice.GroupBy(x => x.Contract);
                    Dictionary<(long,int), PkudatYomanShura> pkudatYomanShuras = new Dictionary<(long, int), PkudatYomanShura>();
                    Dictionary<(long, int), PkudatYomanShura> pkudatYomanShurasBanks = new Dictionary<(long, int), PkudatYomanShura>();
                    pkudatYomanShuras.Add((supplier.SupplierNumberInFinance,2), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key.ToString()));
                    foreach (var row in data)
                    {
                        var contract = contracts.FirstOrDefault(x => x.Id == row.Key);
                        if (contract == null)
                        {
                            break;
                        }
                        var budgets = getContractBudgets(budgetsContracts, row.Key);
                        for (int j = 0; j < budgets.Length; j++)
                        {
                            double amount = row.Sum(x => Convert.ToDouble(x.Amount)) * budgets[j].Precent / 100;
                            PkudatYomanShura pkudatYomanShura = new PkudatYomanShura();
                            if (pkudatYomanShuras.TryGetValue((budgets[j].BudgetId,1), out pkudatYomanShura))
                            {
                                pkudatYomanShura.Schum += amount;
                            }
                            else
                            {
                                pkudatYomanShura = createPkudatYomanBudgetWithOutAmount(summary, supplier, dateOfRegistration, row.First().DateOfValue.ToString("yyyyMMdd"), budgets[j].BudgetId, invoice.Key.ToString());
                                //pkudatYomanShura.SugMaam = 1;// for business client like water corporations null if it's a regular client
                                pkudatYomanShura.Schum = amount;
                                pkudatYomanShuras.Add((budgets[j].BudgetId, 1), pkudatYomanShura);
                            }
                            if (supplier.WithBanks)
                            {                      
                                pkudatYomanShura = new PkudatYomanShura();
                                if (pkudatYomanShurasBanks.TryGetValue((contract.BankAccountInFinance,2), out pkudatYomanShura))
                                {
                                    pkudatYomanShura.Schum += amount;
                                }
                                else
                                {
                                    pkudatYomanShura = createPkudatYomanBankWithOutAmount(summary, supplier, dateOfRegistration, DateOfValueString, contract.BankAccountInFinance, invoice.Key.ToString());
                                    pkudatYomanShura.Schum = amount;
                                    pkudatYomanShurasBanks.Add((contract.BankAccountInFinance, 2), pkudatYomanShura);
                                }
                            }
                        }
                        foreach (var item in row)
                        {
                            item.JournalEntryNumber = supplier.PkudatYomanNumber;
                        }
                    }
                    if (supplier.WithBanks)
                    {
                        pkudatYomanShurasBanks.Add((supplier.SupplierNumberInFinance,1), createPkduatYomanSupplierRow(summary, supplier, dateOfRegistration, DateOfValueString, invoice.Key.ToString()));
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].ZchutChovaInd = 1;
                        //if only one bank we add counter account else it's zero
                        pkudatYomanShurasBanks[(supplier.SupplierNumberInFinance, 1)].MisparCheshbonNegdi = pkudatYomanShurasBanks.Count == 2 ? pkudatYomanShurasBanks.ElementAt(0).Value.MisparCheshbon : 0;
                        pkudatYomanShuras = pkudatYomanShuras.Concat(pkudatYomanShurasBanks).ToDictionary(x => x.Key, x => x.Value);
                    }
                    invoices.Add(Convert.ToInt64(invoice.Key), pkudatYomanShuras);
                }

                List<PkudatYomanShura> shuras = new List<PkudatYomanShura>();
                double budgetSum = 0;
                foreach (var invoice in invoices)
                {
                    foreach ((PkudatYomanShura row, int i) in invoice.Value.Values.Select((value, i) => (value, i)))
                    {
                        budgetSum += row.ZchutChovaInd == 2 ? -1 * row.Schum.Value : row.ZchutChovaInd == 1 ? row.Schum.Value : 0;
                        row.Schum = Math.Round(Convert.ToDouble(row.Schum), 2);
                        shuras.Add(row);
                    }
                }
                return (shuras.ToArray(), budgetSum > -0.01 && budgetSum < 0.01);
            });
        }
        private BudgetContractEntity[] getContractBudgets(BudgetContractEntity[] budgetsContracts, int contract)
        {
            return budgetsContracts.Where(x => x.ContractId == contract).ToArray();
        }
        private PkudatYomanShura createPkudatYomanShuraDefaults()
        {
            return new PkudatYomanShura
            {
                AsmachtaMishnit = 0,
                MahutShura = 0,
                Teur = "",
                SugMatbea = 0,
            };
        }
        private PkudatYomanShura createPkduatYomanSupplierRow(GeneralBillingSummary summary, SupplierEntity supplier, string dateOfRegistration, string DateOfValueString, string invoice)
        {
            PkudatYomanShura pkudatYomanShura = createPkudatYomanShuraDefaults();
            pkudatYomanShura.Teur = new string(("חיוב חודש " + "datetime" + " משלם-" + "clientnumber").ToCharArray().Reverse().ToArray());
            pkudatYomanShura.AsmachtaRashit = invoice.Length >9 ? Convert.ToInt32(invoice.Substring(0,9)) : Convert.ToInt32(invoice);
            pkudatYomanShura.MisparCheshbonit = Convert.ToInt64(invoice.Substring(3));
            pkudatYomanShura.MisparCheshbon = supplier.SupplierNumberInFinance;
            pkudatYomanShura.TaarichRishum = dateOfRegistration;
            pkudatYomanShura.TaarichErech = DateOfValueString;
            pkudatYomanShura.ZchutChovaInd = 2;
            pkudatYomanShura.TaarichCheshbonit = pkudatYomanShura.TaarichErech;
            pkudatYomanShura.MisparCheshbonNegdi = 0;
            pkudatYomanShura.Schum = Convert.ToDouble(summary.TotalDebit);
            return pkudatYomanShura;
        }
        private PkudatYomanShura createPkudatYomanBankWithOutAmount(GeneralBillingSummary summary, SupplierEntity supplier, string dateOfRegistration, string DateOfValueString, long bankInFinance,string invoice)
        {
            PkudatYomanShura pkudatYomanShura = createPkudatYomanShuraDefaults();
            pkudatYomanShura.Teur = new string(("חיוב חודש " + "datetime" + " משלם-" + "clientnumber").ToCharArray().Reverse().ToArray());
            pkudatYomanShura.AsmachtaRashit = invoice.Length > 9 ? Convert.ToInt32(invoice.Substring(0, 9)) : Convert.ToInt32(invoice);
            pkudatYomanShura.MisparCheshbonit = Convert.ToInt64(invoice.Substring(3));
            pkudatYomanShura.MisparCheshbon = bankInFinance;
            pkudatYomanShura.TaarichRishum = dateOfRegistration;
            pkudatYomanShura.TaarichErech = DateOfValueString;
            pkudatYomanShura.MisparCheshbonNegdi = supplier.SupplierNumberInFinance;
            pkudatYomanShura.ZchutChovaInd = 2;
            pkudatYomanShura.TaarichCheshbonit = pkudatYomanShura.TaarichErech;
            pkudatYomanShura.Schum = 0;
            return pkudatYomanShura;
        }
        private PkudatYomanShura createPkudatYomanBudgetWithOutAmount(GeneralBillingSummary summary, SupplierEntity supplier, string dateOfRegistration, string DateOfValueString, long budgetId,string invoice)
        {
            PkudatYomanShura pkudatYomanShura = createPkudatYomanShuraDefaults();
            pkudatYomanShura.Teur = new string(("חיוב חודש " + "datetime" + " משלם-" + "clientnumber").ToCharArray().Reverse().ToArray());
            pkudatYomanShura.AsmachtaRashit = invoice.Length > 9 ? Convert.ToInt32(invoice.Substring(0, 9)) : Convert.ToInt32(invoice);
            pkudatYomanShura.MisparCheshbon = budgetId;
            pkudatYomanShura.MisparCheshbonit = Convert.ToInt64(invoice.Substring(3));
            pkudatYomanShura.TaarichRishum = dateOfRegistration;
            pkudatYomanShura.TaarichErech = DateOfValueString;
            pkudatYomanShura.ZchutChovaInd = 1;
            pkudatYomanShura.TaarichCheshbonit = pkudatYomanShura.TaarichErech;
            pkudatYomanShura.MisparCheshbonNegdi = supplier.SupplierNumberInFinance;
            return pkudatYomanShura;
        }
    }
}
