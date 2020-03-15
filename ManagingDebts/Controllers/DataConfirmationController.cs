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
    [Route("dataConfirmation")]
    [ApiController]
    public class DataConfirmationController : ControllerBase
    {
        private readonly IBezekDataConfirmationService bezekDataConfirmationService;
        private readonly IElectricityDataConfirmationService electricityDataConfirmationService;
        private readonly IPrivateSupplierDataConfirmationService privateSupplierDataConfirmationService;
        private readonly ILogger<DataConfirmationController> log;

        public DataConfirmationController(IBezekDataConfirmationService bezekDataConfirmationService, IElectricityDataConfirmationService electricityDataConfirmationService, IPrivateSupplierDataConfirmationService privateSupplierDataConfirmationService, ILogger<DataConfirmationController> log)
        {
            this.bezekDataConfirmationService = bezekDataConfirmationService;
            this.electricityDataConfirmationService = electricityDataConfirmationService;
            this.privateSupplierDataConfirmationService = privateSupplierDataConfirmationService;
            this.log = log;
        }

        [HttpPost("getDataBySummary")]
        public async Task<IActionResult> GetDataBySummary([FromBody]GeneralBillingSummaryEntity summaryEntity)
        {
            try
            {
                switch (Convert.ToInt32(summaryEntity.SupplierId))
                {
                    case (int)Enums.Suppliers.Bezek:
                        return Ok(await bezekDataConfirmationService.GetDataBySummary(summaryEntity));
                    case (int)Enums.Suppliers.Electricity:
                        return Ok(await electricityDataConfirmationService.GetDataBySummary(summaryEntity));
                    default:
                        return Ok(await bezekDataConfirmationService.GetDataBySummary(summaryEntity));
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("matchBezek")]
        public async Task<IActionResult> MatchBezek([FromBody] BezekInfoEntity[] bezeks)
        {
            try
            {
                return Ok(await bezekDataConfirmationService.MatchBezek(bezeks));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }

        }
        [HttpPost("matchElectricity")]
        public async Task<IActionResult> MatchElectricity([FromBody] ElectricityInfoEntity[] electricities)
        {
            try
            {
                return Ok(await electricityDataConfirmationService.MatchElectricity(electricities));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }

        }
        [HttpPost("matchPrivateSupplier")]
        public async Task<IActionResult> MatchPrivateSupplier([FromBody] PrivateSupplierFileInfoEntity[] privateSuppliers)
        {
            try
            {
                return Ok(await privateSupplierDataConfirmationService.MatchPrivateSupplier(privateSuppliers));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }

        }
    }
}