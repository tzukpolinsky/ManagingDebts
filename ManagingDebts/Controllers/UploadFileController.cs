using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Services;

namespace ManagingDebts.Controllers
{
    [Route("uploadFile")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IBezekFileReaderService bezekFileReaderService;
        private readonly IElectricityFileReaderService electricityFileReaderService;
        private readonly IGeneralBillingSummaryService generalBillingSummaryService;
        private readonly IPrivateSupplierFileReaderService privateSupplierFileReaderService;
        private readonly ILogger<UploadFileController> log;

        public UploadFileController(IBezekFileReaderService bezekFileReaderService, IElectricityFileReaderService electricityFileReaderService, IGeneralBillingSummaryService generalBillingSummaryService, IPrivateSupplierFileReaderService privateSupplierFileReaderService, ILogger<UploadFileController> log)
        {
            this.bezekFileReaderService = bezekFileReaderService;
            this.electricityFileReaderService = electricityFileReaderService;
            this.generalBillingSummaryService = generalBillingSummaryService;
            this.privateSupplierFileReaderService = privateSupplierFileReaderService;
            this.log = log;
        }

        [HttpPost("uploadFile/{customerId}/{supplierId}")]
        public async Task<IActionResult> UploadFile(int customerId, string supplierId, IFormFile file)
        {
            try
            {
                switch (Convert.ToInt32(supplierId))
                {
                    case (int)Enums.Suppliers.Bezek:
                        return Ok(await bezekFileReaderService.GetDataFromExcel(new FileEntity { CustomerId = customerId, SupplierId = supplierId, File = file }));
                    case (int)Enums.Suppliers.Electricity:
                        return Ok(await electricityFileReaderService.GetDataFromExcel(new FileEntity { CustomerId = customerId, SupplierId = supplierId, File = file }));
                    default:
                        return Ok(await privateSupplierFileReaderService.GetDataFromExcel(new FileEntity { CustomerId = customerId, SupplierId = supplierId, File = file }));
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("getAllSummaryByCustomer")]
        public IActionResult GetAllSummaryByCustomer([FromBody] GeneralBillingSummaryEntity generalBillingSummaryEntity)
        {
            try
            {
                var result = generalBillingSummaryService.GetSummaryLines(generalBillingSummaryEntity);
                return Ok(result);
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("deleteDataBySummary")]
        public IActionResult DeleteDataBySummary([FromBody] GeneralBillingSummaryEntity generalBillingSummaryEntity)
        {
            try
            {
                var result = generalBillingSummaryService.Delete(generalBillingSummaryEntity);
                return Ok(result);
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}
