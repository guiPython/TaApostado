using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using Microsoft.Extensions.Logging;
using TaApostado.Extension;
using Microsoft.AspNetCore.Http;
using TaApostado.Exceptions.Services;
using Microsoft.AspNetCore.SignalR;
using TaApostado.Hubs;

namespace TaApostado.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BetController : ControllerBase
    {
        private readonly IServiceBet _serviceBet;
        private readonly ILogger<BetController> _logger;
        private readonly IHubContext<GameHub> _gameHub;
        public BetController(IServiceBet serviceBet, ILogger<BetController> logger, IHubContext<GameHub> gameHub)
        {
            _serviceBet = serviceBet;
            _logger = logger;
            _gameHub = gameHub;
        }

        [HttpPost("{challenge_id:guid}/bid/{bid}")]
        public async Task<ActionResult<OutputModelBet>> CreateBet([FromRoute] Guid challenge_id, decimal bid)
        {
            try
            {
                var user_id = this.ReadToken();
                var bet = await _serviceBet.CreateBet(Guid.Parse(user_id), challenge_id, bid);

                var challenge = await _serviceBet.SelectChallenge(Guid.Parse(bet.id));
                await _gameHub.Clients.Group(challenge_id.ToString()).SendAsync("Send", challenge);

                return Ok(bet);
            }
            catch (Exception e) when(e is ChallengeNotExistsException || e is UserNotExistsException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e) when(e is ChallengeIsNotAtiveException || e is ChallengedEqualsBettorException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e) when(e is BidOutRangeUserAmountException || e is BidOutRangeChallengeBidException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status422UnprocessableEntity);
            } 
            catch (Exception e)
            {
                return this.Error(_logger,e,bid,"Create Bet",StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{bet_id:guid}/bid/{bid}")]
        public async Task<ActionResult> UpdateBetBid([FromRoute] Guid bet_id, decimal bid)
        {
            try
            {
                var user_id = this.ReadToken();
                await _serviceBet.UpdateBetBid(Guid.Parse(user_id), bet_id, bid);

                var challenge = await _serviceBet.SelectChallenge(bet_id);
                await _gameHub.Clients.Group(challenge.id).SendAsync("Send", challenge);

                return Ok();
            }
            catch (Exception e) when(e is UserNotExistsException || e is ChallengeNotExistsException || e is BetNotExistsException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e) when(e is ChallengedEqualsBettorException || e is ChallengeIsNotAtiveException || e is ChallengeNotBelongsToUserException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e) when(e is BidOutRangeChallengeBidException || e is BidOutRangeUserAmountException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, bid, "Update Bet", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{bet_id:guid}")]
        public async Task<ActionResult> DeleteBet([FromRoute] Guid bet_id)
        {
            try
            {
                var user_id = this.ReadToken();
                await _serviceBet.DeleteBet(Guid.Parse(user_id), bet_id);
                return Ok();
            }
            catch (Exception e) when (e is UserNotExistsException || e is ChallengeNotExistsException || e is BetNotExistsException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e) when (e is ChallengedEqualsBettorException || e is ChallengeIsNotAtiveException || e is BetNotBelongsToUserException)
            {
                return this.ServiceError(e.Message, StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                return this.Error(_logger, e, bet_id, "Delete Bet", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
