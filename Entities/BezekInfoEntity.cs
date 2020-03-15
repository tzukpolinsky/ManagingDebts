using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class BezekInfoEntity
    {
        public string ClientNumber { get; set; }
        public int PayerNumberBezek { get; set; }
        public string DepartmentNumber { get; set; }
        public int Contract { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public int ConsumptionAmount { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public string OriginalClient { get; set; }
        public int OriginalPayer { get; set; }
        public double DiscountPrecent { get; set; }
        public string CallTime { get; set; }
        public int? CallsAmount { get; set; }
        public decimal CallRate { get; set; }
        public string FreeTimeUsage { get; set; }
        public string TimePeriodText { get; set; }
        public string ServiceType { get; set; }
        public string SecondaryServiceType { get; set; }
        public string HebServiceType { get; set; }
        public string FreeTimeUsageSupplier { get; set; }
        public int GeneralSummaryRowId { get; set; }
        public bool IsMatched { get; set; }
        public int RowId { get; set; }
        public int? TaxRate { get; set; }
        public int CustomerId { get; set; }

    }
}
