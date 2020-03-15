using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBezekDataConfirmationService
    {
         Task<Dictionary<string, BezekInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity);
         Task<Dictionary<string, BezekInfoEntity[]>> GetDataByDate(GeneralBillingSummaryEntity summaryEntity);
        Task<bool> MatchBezek(BezekInfoEntity[] bezeks);
    }
}
