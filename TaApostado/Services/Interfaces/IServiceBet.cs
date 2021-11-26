using System;
using System.Threading.Tasks;
using TaApostado.DTOs.OutputModels;

namespace TaApostado.Services.Interfaces
{
    public interface IServiceBet
    {
        public Task<OutputModelBet> CreateBet(Guid user_id, Guid challenge_id, decimal bid);
        public Task<OutputModelBet> SelectBet(Guid bet_id);
        public Task<OutputModelChallenge> SelectChallenge(Guid challenge_id);
        public Task UpdateBetBid(Guid user_id, Guid bet_id, decimal bid);
        public Task DeleteBet(Guid user_id, Guid bet_id);
    }
}
