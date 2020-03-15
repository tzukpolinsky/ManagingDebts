using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using System;
namespace ManagingDebts.Controllers
{
    [Route("supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISuppliersService suppliersService;
        private readonly ILogger<SupplierController> log;

        public SupplierController(ISuppliersService suppliersService, ILogger<SupplierController> log)
        {
            this.suppliersService = suppliersService;
            this.log = log;
        }

        [HttpPost("getSuppliersByCustomer")]
        public IActionResult GetSuppliersByCustomer([FromBody] CustomerEntity customer)
        {
            try
            {
                return Ok(suppliersService.GetSuppliersByCustomer(customer));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
          
        }
        [HttpPost("addSupplier")]
        public IActionResult AddSupplier([FromBody] SupplierEntity supplier)
        {
            try
            {
                return Ok(suppliersService.Add(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message+"\nin:" + e.StackTrace);
                return Problem(null);
            }
           
        }
        [HttpPost("editSupplier")]
        public IActionResult EditSupplier([FromBody] SupplierEntity supplier)
        {
            try
            {
                return Ok(suppliersService.Edit(supplier));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}