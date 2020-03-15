using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class Customers
    {
        public Customers()
        {
            Contracts = new HashSet<Contracts>();
            GeneralBillingSummary = new HashSet<GeneralBillingSummary>();
            Users = new HashSet<Users>();
        }

        public int FinanceId { get; set; }
        public string MgaName { get; set; }
        public bool IsActive { get; set; }
        public bool IsBusiness { get; set; }

        public virtual ICollection<Contracts> Contracts { get; set; }
        public virtual ICollection<GeneralBillingSummary> GeneralBillingSummary { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
