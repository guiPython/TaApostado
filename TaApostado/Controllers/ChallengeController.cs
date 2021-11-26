using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Exceptions.Services;
using TaApostado.Extension;
using Microsoft.AspNetCore.SignalR;
using TaApostado.Hubs;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace TaApostado.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengeController : ControllerBase
    {
        private readonly IServiceChallenge _service;
        private readonly ILogger<ChallengeController> _logger;
        private readonly IHubContext<GameHub> _gameHub;
        public ChallengeController(IServiceChallenge service, ILogger<ChallengeController> logger, IHubContext<GameHub> gameHub)
        {
            _service = service;
            _logger = logger;
            _gameHub = gameHub;
        }

        /// <summary>
        /// Create Challenge
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /challenge
        ///     
        /// </remarks>
        /// <param name="challenge">Input Model Challenge</param>
        /// <response code="200">Challenge created</response>
        /// <response code="400">User not exists</response>
        /// <response code="500">Server error</response>
        [HttpPost]
        public async Task<ActionResult<OutputModelChallenge>> CreateChallenge([FromBody] InputModelChallenge challenge)
        {
            try
            {
                var user_id = this.ReadToken();
                var register = await _service.CreateChallenge(Guid.Parse(user_id), challenge);
                return Ok(register);
            }
            catch (ArgumentException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, challenge, "Create Challenge", StatusCodes.Status500InternalServerError);
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
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateChallenge([FromRoute] Guid id, [FromBody] InputModelChallenge challenge)
        {
            try
            {
                var user_id = this.ReadToken();
                await _service.UpdateChallenge(id, Guid.Parse(user_id), challenge);

                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ChallengeBidIsInvalidException || e is ChallengeQuotaIsInvalidException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, challenge, "Update Challenge", StatusCodes.Status500InternalServerError);
            }  
        }

        /// <summary>
        /// Challenge update quota by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}/quota/{quota}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <param name="name">Challenge quota</param>
        /// <response code="200">Challenge name updated</response>
        /// <response code="401">User unauthorized update challenge name</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id:guid}/name/{name}")]
        public async Task<ActionResult> UpdateChallengeName([FromRoute] Guid id, [FromRoute] string name)
        {
            try
            {
                if (name.Length < 10 || name.Length > 30 || string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name length must be in range(10, 30)");

                var user_id = this.ReadToken();
                await _service.UpdateChallengeName(id, Guid.Parse(user_id), name);

                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);

                return Ok();
            }
            catch (Exception e) when(e is ArgumentException || e is ChallengeIsSuspendedException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, name, "Update Challenge name", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge update quota by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}/quota/{quota}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <param name="theme">Challenge theme</param>
        /// <response code="200">Challenge theme updated</response>
        /// <response code="401">User unauthorized update challenge quota</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id:guid}/theme/{theme}")]
        public async Task<ActionResult> UpdateChallengeTheme([FromRoute] Guid id, [FromRoute] string theme)
        {
            try
            {
                if (theme.Length < 3 || theme.Length > 12 || string.IsNullOrWhiteSpace(theme))
                    throw new ArgumentException("Theme length must be in range(3, 12)");

                var user_id = this.ReadToken();
                await _service.UpdateChallengeTheme(id, Guid.Parse(user_id), theme);

                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);

                return Ok();
            }
            catch (Exception e) when(e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, theme, "Update Challenge theme", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge update description by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}/description/{description}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <param name="description">Challenge description</param>
        /// <response code="200">Challenge description updated</response>
        /// <response code="401">User unauthorized update challenge description</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id:guid}/description/{description}")]
        public async Task<ActionResult> UpdateChallengeDescription([FromRoute] Guid id, [FromRoute] string description)
        {
            try
            {
                if (description.Length < 30 || description.Length > 80 || string.IsNullOrWhiteSpace(description))
                    throw new ArgumentException("Description length must be in range(30, 80)");

                var user_id = this.ReadToken();
                await _service.UpdateChallengeDescription(id, Guid.Parse(user_id), description);

                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);

                return Ok();
            }
            catch (Exception e) when(e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, description, "Update Challenge description", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge update bid by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}/bid/{bid}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <param name="bid">Challenge quota</param>
        /// <response code="200">Challenge bid updated</response>
        /// <response code="401">User unauthorized update challenge bid</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id:guid}/bid/{bid}")]
        public async Task<ActionResult> UpdateChallengeBid([FromRoute] Guid id, [FromRoute] int bid)
        {
            try
            {
                var user_id = this.ReadToken();

                if (bid <= 0) throw new ArgumentException("The field value is invalid.");

                await _service.UpdateChallengeBid(id, Guid.Parse(user_id), bid);
                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, bid, "Update Challenge bid", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge update quota by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}/quota/{quota}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <param name="quota">Challenge quota</param>
        /// <response code="200">Challenge quota updated</response>
        /// <response code="401">User unauthorized update challenge quota</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id:guid}/quota/{quota}")]
        public async Task<ActionResult> UpdateChallengeQuota([FromRoute] Guid id, [FromRoute] int quota)
        {
            try
            {
                var user_id = this.ReadToken();

                if (quota <= 0) throw new ArgumentException("The field bid is invalid.");

                await _service.UpdateChallengeQuota(id, Guid.Parse(user_id),quota);
                var register = await _service.SelectChallenge(id);
                await _gameHub.Clients.Group(id.ToString()).SendAsync("Send", register);
                return Ok();
            }
            catch (Exception e) when(e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, quota, "Update Challenge quota", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge start execution by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <response code="200">Challenge start execution</response>
        /// <response code="401">User unauthorized start challenge execution</response>
        /// <response code="500">Server error</response>
        [HttpPatch("execute/{id:guid}")]
        public async Task<ActionResult> ExecuteChallenge([FromRoute] Guid id)
        {
            try
            {
                var user_id = this.ReadToken();
                await _service.UpdateChallengeToExecution(id, Guid.Parse(user_id));
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, id, "Suspend Challenge", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Challenge start votation by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <response code="200">Challenge start votation</response>
        /// <response code="401">User unauthorized start challenge votation</response>
        /// <response code="500">Server error</response>
        [HttpPatch("vote/{id:guid}")]
        public async Task<ActionResult> VoteChallenge([FromRoute] Guid id)
        {
            try
            {
                var user_id = this.ReadToken();
                await _service.UpdateChallengeToVotation(id, Guid.Parse(user_id));
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, id, "Suspend Challenge", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Suspend Challenge by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PATCH /challenge/{id}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <response code="200">Challenge suspended</response>
        /// <response code="401">User unauthorized suspend challenge</response>
        /// <response code="500">Server error</response>
        [HttpPatch("suspend/{id:guid}")]
        public async Task<ActionResult> SuspendChallenge([FromRoute] Guid id)
        {
            try
            {
                var user_id = this.ReadToken();
                await _service.UpdateChallengeToSuspended(id, Guid.Parse(user_id));
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, id, "Suspend Challenge", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Challenge by Id
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     DELETE /challene/{id}
        ///     
        /// </remarks>
        /// <param name="id">Challenge Id</param>
        /// <response code="200">Challenge deleted</response>
        /// <response code="401">User unauthorized delete challenge</response>
        /// <response code="500">Server error</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteChallenge([FromRoute] Guid id)
        {
            try
            {
                var user_id = this.ReadToken();
                await _service.UpdateChallengeToDeleted(id, Guid.Parse(user_id));
                return Ok();
            }
            catch (Exception e) when (e is ChallengeIsSuspendedException || e is ArgumentException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status304NotModified);
            }
            catch (ChallengeNotBelongsToUserException e)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, id, "Delete Challenge", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
