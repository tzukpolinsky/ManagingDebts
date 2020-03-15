using System;
using System.Collections.Generic;

namespace ManagingDebts
{
    public partial class CustomersSuppliers
    {
        public string SupplierId { get; set; }
        public int CustomerId { get; set; }
        public string SupplierNumberInFinance { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Suppliers Supplier { get; set; }
    }
}
