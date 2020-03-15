using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IDashboardService
    {
        ChartEntity[] GetContractChartInfo(SupplierEntity supplier);
        ChartEntity[] GetBudgetChartInfo(SupplierEntity supplier);
    }
}
