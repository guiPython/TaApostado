using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaApostado.Hubs;
using TaApostado.Persistence;
using TaApostado.Entities.Enums;
using TaApostado.Entities;

namespace TaApostado.HostedServices
{
    public class GameHostedService : BackgroundService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<GameHub> _gameHub;
        private readonly ILogger<GameHostedService> _logger;

        public GameHostedService(ILogger<GameHostedService> logger, IHubContext<GameHub> gameHub, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _gameHub = gameHub;
            _scopeFactory = scopeFactory;
        }

        private async Task DeleteChallenges(DBContext dbContext)
        {
            foreach(var challenge in dbContext.Challenges)
            {
                if (challenge.Status == ChallengeStatus.SUSPENDED && challenge.Bets.Count == 0)
                {
                    await _gameHub.Clients.Group(challenge.Id.ToString()).SendAsync("Send",$"The challenge {challenge.Id} has been deleted {DateTime.Now}");
                    dbContext.Challenges.Remove(challenge);
                    
                    await dbContext.SaveChangesAsync();
                }
            }
   
        }
        private async Task SuspendChallenges(DBContext dbContext)
        {
            foreach (var challenge in dbContext.Challenges)
            {
                var time = challenge.TimeOpen + challenge.TimeExecution + Challenge.GetTimeVotation();
                if (challenge.Status == ChallengeStatus.VOTATION && challenge.CreatedAt.AddMinutes(time) <= DateTime.Now)
                {
                    await _gameHub.Clients.Group(challenge.Id.ToString()).SendAsync("Send", $"The challenge {challenge.Id} has been supended {DateTime.Now}");
                    challenge.SetAsSuspended();
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        private async Task VotationChallenges(DBContext dbContext)
        {
            foreach (var challenge in dbContext.Challenges)
            {
                var time = challenge.TimeOpen + challenge.TimeExecution;
                if (challenge.Status == ChallengeStatus.EXECUTION && challenge.CreatedAt.AddMinutes(time) <= DateTime.Now)
                {
                    await _gameHub.Clients.Group(challenge.Id.ToString()).SendAsync("Send", $"The challenge {challenge.Id} has been votation {DateTime.Now}");
                    challenge.SetAsVotation();
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task ExecutionChallenges(DBContext dbContext)
        {
            foreach (var challenge in dbContext.Challenges)
            {
                var time = challenge.TimeOpen;
                if (challenge.Status == ChallengeStatus.ACTIVE && challenge.CreatedAt.AddMinutes(time) <= DateTime.Now)
                {
                    await _gameHub.Clients.Group(challenge.Id.ToString()).SendAsync("Send", $"The challenge {challenge.Id} has been execution {DateTime.Now}");
                    challenge.SetAsExecution();
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running Host Service Game");

                using(var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
                    await DeleteChallenges(dbContext);
                    await SuspendChallenges(dbContext);
                    await VotationChallenges(dbContext);
                    await ExecutionChallenges(dbContext);
                }

                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
