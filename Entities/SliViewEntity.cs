using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class SliViewEntity
    {
        public GeneralBillingSummaryEntity Summary { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public UserEntity User { get; set; }
    }
}
