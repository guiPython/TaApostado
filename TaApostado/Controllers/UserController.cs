using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaApostado.Exceptions.Services;
using Microsoft.AspNetCore.Http;
using TaApostado.Extension;

namespace TaApostado.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServiceUser _service;
        private readonly ILogger<UserController> _logger;
        public UserController(IServiceUser service, ILogger<UserController> logger)
        {
            _service = service;
            _logger = logger;
        }
         /// <summary>
         /// Get User by Id
         /// </summary>
         /// <remarks>
         ///    GET /user
         /// </remarks>
         /// <response code="200">User exists</response>
         /// <response code="204">User not exists</response>
         /// <response code="500">Server error</response>
         /// <returns>User by Id</returns>
        [HttpGet]
        public async Task<ActionResult<OutputModelUser>> GetUser()
        {
            try
            {
                var id = User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
                if (id is null) throw new UserNotExistsException();

                var result = await _service.Find(Guid.Parse(id));
                if (result is null) throw new UserNotExistsException();

                return Ok(result);
            }
            catch(UserNotExistsException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status204NoContent);
            }
            catch(Exception e)
            {
                return this.Error(_logger, e, "id", "Find User", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PUT /user
        ///     {
        ///       "name": "Alex",
        ///       "lastName": "Rodrigez",
        ///       "cpf": xxx.xxx.xxx-xx,
        ///       "email": "test@test.com",
        ///       "password": "test1234@"
        ///      }
        ///     
        /// </remarks>
        /// <response code="200">User updated</response>
        /// <response code="204">User not updated</response>
        /// <response code="500">Server error</response>
        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] InputModelUser user)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
                if (id is null) throw new UserNotExistsException();

                await _service.Update(Guid.Parse(id), user);
                return Ok();
            }
            catch (UserNotExistsException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status204NoContent);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, user, "Update User", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update User Name and Last name
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /user?name="Jorge"&amp;lastName="Gomes"
        ///    
        /// </remarks>
        /// <param name="name">The new name for user</param>
        /// <param name="lastName">The new last name for user</param>
        /// <response code="200">User name and last name updated</response>
        /// <response code="204">User name or last name not updated</response>
        /// <response code="500">Server error</response>
        [HttpPatch("name={name}&lastName={lastName}")]
        public async Task<ActionResult> UpdateUserNameAndLastName([FromQuery] string name, [FromRoute] string lastName)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
                if (id is null) throw new UserNotExistsException();
                if (name is null || lastName is null) throw new ArgumentNullException();

                await _service.Update(Guid.Parse(id), name, lastName);
                return Ok();
            }
            catch (Exception e) when ( e is UserNotExistsException || e is ArgumentNullException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, name + " " + lastName, "Update User Name and LastName", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update User password
        /// </summary>
        /// <remarks>
        /// Sample  Request:
        /// 
        ///     PATCH /user?password="@Teste123"
        ///     
        /// </remarks>
        /// <param name="password">The new password for user</param>
        /// <response code="200">User password updated</response>
        /// <response code="400">User password not updated</response>
        /// <response code="500">Server error</response>
        [HttpPatch("password={password}")]
        public async Task<ActionResult> UpdateUserPassword([FromRoute] string password)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
                if (id is null) throw new UserNotExistsException();
                if (password is null) throw new ArgumentNullException();

                await _service.Update(Guid.Parse(id), password);
                return Ok();
            }
            catch(Exception e) when  (e is UserNotExistsException || e is ArgumentNullException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, "id","Update User Pasword", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete User by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     DELETE /user
        ///     
        /// </remarks>
        /// <response code="200">User deleted</response>
        /// <response code="400">User not exists</response>
        /// <response code="500">Server error</response>
        [HttpDelete]
        public async Task<ActionResult> DeleteUser()
        {
            try
            {
                var id = User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid).Value;
                if (id is null) throw new UserNotExistsException();

                await _service.Delete(Guid.Parse(id));
                return Ok();
            }
            catch (UserNotExistsException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, "id", "Delete User", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
