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
    [Route("mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly ILogger<MailController> log;
        private readonly IMailService mailService;

        public MailController(ILogger<MailController> log, IMailService mailService)
        {
            this.log = log;
            this.mailService = mailService;
        }
        [HttpPost("reportError")]
        public IActionResult ReportError([FromBody] UserEntity user)
        {
            try
            {
                return Ok(mailService.SendErrorEmail(user));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        } 
        [HttpPost("reportAccessDenied")]
        public IActionResult ReportAccessDenied([FromBody] UserEntity user)
        {
            try
            {
                return Ok(mailService.SendAccessDeniedEmail(user));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        } 
        [HttpPost("manualLocalStorageChange")]
        public IActionResult ManualLocalStorageChange([FromBody] UserEntity user)
        {
            try
            {
                return Ok(mailService.SendSystemSabotage(user,"שינוי ערכים בlocale stoarge"));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}