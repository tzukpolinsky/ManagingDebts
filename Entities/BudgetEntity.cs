using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class BudgetEntity
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }
        public string Name { get; set; }
    }
}
