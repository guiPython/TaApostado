using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Exceptions.Services;
using Microsoft.AspNetCore.Authorization;
using TaApostado.Extension;

namespace TaApostado.Controllers
{   
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IServiceLogin _serviceLogin;
        public LoginController(ILogger<LoginController> logger, IServiceLogin serviceLogin)
        {
            _logger = logger;
            _serviceLogin = serviceLogin;
        }


        /// <summary>
        /// SignIn
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /signIn
        ///     {
        ///         "email": "test@test.com",
        ///         "password": "test1234@"
        ///     }
        ///     
        /// </remarks>
        /// <param name="login"></param>
        /// <returns>User and JWT</returns>
        /// <response code="200">Return the user and token for access</response>
        /// <response code="400">If user not exists</response>
        /// <response code="500">If server throws exceptions</response>
        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<ActionResult<OutputModelLogin>> Post([FromBody] InputModelLogin login)
        {
            try
            {
                var output = await _serviceLogin.SignIn(login);
                return Ok(output);
            }
            catch (UserNotExistsException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch(Exception e)
            {
                return this.Error(_logger, e, login, "SignIn", StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// SignUp
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /signUp
        ///     {
        ///         "name": "Alex",
        ///         "lastName": "Rodrigez",
        ///         "cpf": xxx.xxx.xxx-xx,
        ///         "email": "test@test.com",
        ///         "password": "test1234@"
        ///     }
        ///     
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>User and JWT</returns>
        /// <response code="200">Return the user and token for access</response>
        /// <response code="400">If some field of body request is invalid</response>
        /// <response code="422">If user already exists</response>
        /// <response code="500">If server throws exceptions</response>
        [AllowAnonymous]
        [HttpPost("signUp")]
        public async Task<ActionResult<OutputModelLogin>> Post([FromBody] InputModelUser user)
        {
            try
            {
                var output = await _serviceLogin.SignUp(user);
                return Ok(output);
            }
            catch(UserAlreadyExistsException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch(Exception e)
            {
                return this.Error(_logger, e, user, "SignUp", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
