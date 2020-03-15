using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPrivateSupplierDataConfirmationService
    {
        Task<Dictionary<string, PrivateSupplierFileInfoEntity[]>> GetDataBySummary(GeneralBillingSummaryEntity summaryEntity);
        Task<bool> MatchPrivateSupplier(PrivateSupplierFileInfoEntity[] privates);
    }
}
