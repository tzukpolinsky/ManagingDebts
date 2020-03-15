using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class SupplierEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public bool IsEnable { get; set; }
        public long SupplierNumberInFinance { get; set; }
        public bool WithBanks { get; set; }
        public int PkudatYomanNumber { get; set; }

    }
}
