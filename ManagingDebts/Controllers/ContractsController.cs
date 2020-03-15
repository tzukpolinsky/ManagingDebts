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
    [Route("contracts")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractsService contractsService;
        private readonly IBudgetContractService budgetContractService;
        private readonly ILogger<ContractsController> log;

        public ContractsController(IContractsService contractsService, IBudgetContractService budgetContractService, ILogger<ContractsController> log)
        {
            this.contractsService = contractsService;
            this.budgetContractService = budgetContractService;
            this.log = log;
        }

        [HttpPost("getContractsBySupplier")]
        public async Task<IActionResult> GetContractsBySupplier([FromBody]SupplierEntity supplier)
        {
            try
            {
                return Ok(await contractsService.GetBySupplier(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }

        [HttpPost("addContract")]
        public IActionResult AddContract([FromBody] ContractEntity contract)
        {
            try
            {
                return Ok(contractsService.Add(contract));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("removeContract")]
        public IActionResult RemoveContract([FromBody] ContractEntity contract)
        {
            try
            {
                return Ok(contractsService.Delete(contract));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("editContract")]
        public IActionResult EditContract([FromBody] ContractEntity contract)
        {
            try
            {
                return Ok(contractsService.Edit(contract));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}