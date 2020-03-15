using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class PrivateSupplierFileInfoEntity
    {
        public string SupplierId { get; set; }
        public int CustomerId { get; set; }
        public int RowId { get; set; }
        public int GeneralRowId { get; set; }
        public long Invoice { get; set; }
        public int Contract { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfValue { get; set; }
        public int JournalEntryNumber { get; set; }
        public int TaxRate { get; set; }
        public decimal AmountAfterTax { get; set; }
        public bool IsMatched { get; set; }

    }
}
