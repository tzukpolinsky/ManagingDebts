using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public static class Enums
    {

        public enum Suppliers
        {
            Bezek = 520031931,
            Electricity = 520000472,
        }
        public enum TaxRate
        {
            CurrentTaxRate = 17
        }
        public enum Commands
        {
            Debit = 1,
            Credit = 2,
            Debit_Credit = 3,
            Credit_Debit = 4,
        }
    }
}
