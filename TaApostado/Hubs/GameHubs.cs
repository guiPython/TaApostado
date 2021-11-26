using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TaApostado.Hubs.Interfaces;
using TaApostado.Persistence;
using System.Linq;
using System.Security.Claims;
using System;
using TaApostado.Entities;
using TaApostado.DTOs.OutputModels;
using Microsoft.EntityFrameworkCore;

namespace TaApostado.Hubs
{
    [Authorize]
    public class GameHub : Hub, IGameHub
    {
        private readonly DBContext _repository;
        public GameHub(DBContext repository)
        {
            _repository = repository;
        }

        private async Task<Challenge> GetChallenge(string id_challenge)
        {
            var challenge = await _repository.Challenges.Include(c=> c.Bets).FirstOrDefaultAsync(c => c.Id == Guid.Parse(id_challenge));
            return challenge;
        }
        
        public async Task AddToGame(string id_challenge)
        {
            
            var challenge = await GetChallenge(id_challenge);
            if (challenge.Status == Entities.Enums.ChallengeStatus.ACTIVE)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, id_challenge);
                var user = await _repository.Users.FindAsync(Guid.Parse(Context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value));
                await Clients.Group(id_challenge).SendAsync("Send", $"{user.Name} has joined the in game {id_challenge}.");
                await Clients.Client(Context.ConnectionId).SendAsync("Send", OutputModelChallenge.ChallengeToOutputChallenge(challenge));
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Send", "Can't add user in game, because challenge status isn't active");
            }   
        }

        public async Task RemoveFromGame(string id_challenge)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id_challenge);
            var user = await _repository.Users.FindAsync(Guid.Parse(Context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Sid).Value));
            await Clients.Group(id_challenge).SendAsync("Send", $"{user.Name} has left the game {id_challenge}.");
        }

    }
}
