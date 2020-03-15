using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IElectricityDataConfirmationService
    {
        Task<Dictionary<string, ElectricityInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity);
        Task<bool> MatchElectricity(ElectricityInfoEntity[] electricities);
    }
}
