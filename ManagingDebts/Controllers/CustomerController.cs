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
    [Route("customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomersService customersService;
        private readonly ILogger<CustomerController> log;

        public CustomerController(ICustomersService customersService, ILogger<CustomerController> log)
        {
            this.customersService = customersService;
            this.log = log;
        }

        [HttpGet("getAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            try
            {
                return Ok(customersService.GetAll());
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }

        }

        [HttpPost("getByUser")]
        public IActionResult GetByUser([FromBody]UserEntity user)
        {
            try
            {
                return Ok(customersService.GetByUser(user));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }

        }
    }
}