using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class ElectricityInfoEntity
    {
        public int Bank { get; set; }
        public int Branch { get; set; }
        public DateTime BillCreatingDate { get; set; }
        public int NumberOfCreditDays { get; set; }
        public decimal AmountAfterTax { get; set; }
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
    }
}
