using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManagingDebts.Controllers
{
    [Route("bankAccounts")]
    public class BankAccountController : Controller
    {
        private readonly IBankAccountsService bankAccountsService;
        private readonly ILogger<BankAccountController> log;

        public BankAccountController(IBankAccountsService bankAccountsService, ILogger<BankAccountController> log)
        {
            this.bankAccountsService = bankAccountsService;
            this.log = log;
        }
        [HttpPost("getBySupplier")]
        public IActionResult GetByCustomer([FromBody]SupplierEntity supplier)
        {
            try
            {
                var banks = bankAccountsService.GetBySupplier(supplier);
                return Ok(banks);
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("addBank")]
        public IActionResult AddBank([FromBody]BankAccountEntity bank)
        {
            try
            {
                return Ok(bankAccountsService.Add(bank));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("deleteBank")]
        public IActionResult DeleteBank([FromBody]BankAccountEntity bank)
        {
            try
            {
                return Ok(bankAccountsService.Delete(bank));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }

    }
}
