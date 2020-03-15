using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class ElectricityFileInfo
    {
        public int BankCode { get; set; }
        public int BankBranch { get; set; }
        public DateTime BillCreatingDate { get; set; }
        public int NumberOfCreditDays { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ConsumerAddress { get; set; }
        public string ConsumerName { get; set; }
        public int BankAccount { get; set; }
        public int BankAccountType { get; set; }
        public int MonthOfLastInvoice { get; set; }
        public int YearOfLastInvoice { get; set; }
        public int ConsumerNumber { get; set; }
        public int Contract { get; set; }
        public long Invoice { get; set; }
        public int CustomerId { get; set; }
        public int GeneralRowId { get; set; }
        public int RowId { get; set; }
        public int JournalEntryNumber { get; set; }
        public bool IsMatched { get; set; }

        public virtual GeneralBillingSummary GeneralRow { get; set; }
    }
}
