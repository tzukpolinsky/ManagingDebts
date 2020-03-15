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
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly ILogger<UsersController> log;

        public UsersController(IUsersService usersService, ILogger<UsersController> log)
        {
            this.usersService = usersService;
            this.log = log;
        }
        [HttpPost("createUser")]
        public IActionResult CreateUser([FromBody]UserEntity user)
        {
            try
            {
                return Ok(usersService.Create(user));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(usersService.GetAll());
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserEntity user)
        {
            try
            {
                var result = usersService.Login(user);
                return Ok(new { result.isSuccess, result.msg, result.user });
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("editUser")]
        public IActionResult EditUser([FromBody] UserEntity user)
        {
            try
            {
                return Ok(usersService.EditUser(user));
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
        [HttpPost("changePassword")]
        public IActionResult ChangePassword([FromBody] UserEntity[] users)
        {
            try
            {
                var result = usersService.ChangePassword(users);
                return Ok(new { result.isSuccess, result.msg});
            }
            catch (Exception e)
            {
                log.LogError(e.Message + "\nin:" + e.StackTrace);
                return Problem(null);
            }
        }
    }
}