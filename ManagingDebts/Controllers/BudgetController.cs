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
    [Route("budgets")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetsService budgetsService;
        private readonly ILogger<BudgetController> log;

        public BudgetController(IBudgetsService budgetsService, ILogger<BudgetController> log)
        {
            this.budgetsService = budgetsService;
            this.log = log;
        }
        [HttpPost("getBySupplier")]
        public IActionResult GetBySupplier([FromBody]SupplierEntity supplier)
        {
            try
            {
                return Ok(budgetsService.GetBySupplier(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("getBudgetsFromFinance")]
        public async Task<IActionResult> GetBudgetsFromFinance([FromBody]BudgetEntity customer)
        {
            try
            {
                return Ok( await budgetsService.GetBudgetsFromFinance(customer));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("convertFileToBudgetsEntities")]
        public async Task<IActionResult> ConvertFileToBudgetsEntities(IFormFile file)
        {
            try
            {
                return Ok(await budgetsService.ConvertFileToEntites(file));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("addBudgets")]
        public IActionResult AddBudgets([FromBody] BudgetEntity[] budgets)
        {
            try
            {
                return Ok(budgetsService.AddMany(budgets));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("addBudget")]
        public IActionResult AddBudget([FromBody] BudgetEntity budget)
        {
            try
            {
                return Ok(budgetsService.Add(budget));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("removeBudget")]
        public IActionResult RemoveBudget([FromBody]BudgetEntity budget)
        {
            try
            {
                return Ok(budgetsService.Delete(budget));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("editBudget")]
        public IActionResult EditBudget([FromBody]BudgetEntity budget)
        {
            try
            {
                return Ok(budgetsService.Edit(budget));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("uploadRelations/{customerId}/{supplierId}")]
        public IActionResult UploadRelations(int customerId, string supplierId, IFormFile file)
        {
            try
            {
                return Ok(budgetsService.AddRelationshipByFile(new FileEntity { CustomerId = customerId, SupplierId = supplierId, File = file }));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}