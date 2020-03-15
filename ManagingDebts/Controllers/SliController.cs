using System;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Services;

namespace ManagingDebts.Controllers
{
    [Route("Sli")]
    [ApiController]
    public class SliController : ControllerBase
    {
        private readonly ISliService sliService;
        private readonly ILogger<SliController> log;

        public SliController(ISliService sliService, ILogger<SliController> log)
        {
            this.sliService = sliService;
            this.log = log;
        }

        [HttpPost("createSli")]
        public IActionResult CreateSli([FromBody] SliViewEntity sli)
        {
            try
            {
                var result = sliService.CreatePkudatYoman(sli.Summary, sli.DateOfRegistration, sli.User);
                return Ok(new { result.isSuccess, result.msg });
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("displaySli")]
        public IActionResult DisplaySli([FromBody] SliViewEntity sli)
        {
            try
            {
                var result = sliService.DisplayPkudatYoman(sli.Summary, sli.DateOfRegistration);
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