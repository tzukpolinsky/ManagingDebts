using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace ManagingDebts.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        private readonly ILogger<BankAccountController> log;

        public DashboardController(IDashboardService dashboardService, ILogger<BankAccountController> log)
        {
            this.dashboardService = dashboardService;
            this.log = log;
        }

        [HttpPost("getContractsInfo")]
        public IActionResult GetContractsInfo([FromBody] SupplierEntity supplier)
        {
            try
            {
                return Ok(dashboardService.GetContractChartInfo(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("getBudgetsInfo")]
        public IActionResult GetBudgetsInfo([FromBody] SupplierEntity supplier)
        {
            try
            {
                return Ok(dashboardService.GetBudgetChartInfo(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}