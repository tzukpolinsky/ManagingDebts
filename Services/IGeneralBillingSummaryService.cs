using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IGeneralBillingSummaryService
    {
        GeneralBillingSummaryEntity[] GetSummaryLines(GeneralBillingSummaryEntity generalBillingSummaryEntity);
        bool Delete(GeneralBillingSummaryEntity generalBillingSummaryEntity);
    }
}
