using System;
using System.Threading.Tasks;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Entities;
using TaApostado.Entities.Enums;
using TaApostado.Persistence;
using TaApostado.Exceptions.Services;
using Microsoft.EntityFrameworkCore;

namespace TaApostado.Services
{
    public class ServiceBet : IServiceBet
    {
        private readonly DBContext _dbContext;
        public ServiceBet(DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OutputModelBet> CreateBet(Guid user_id, Guid challenge_id, decimal bid)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);
            if (register is null) throw new ChallengeNotExistsException();

            if (register.Status != ChallengeStatus.ACTIVE)
                throw new ChallengeIsNotAtiveException();

            if (register.IdChallenged == user_id)
                throw new ChallengedEqualsBettorException();

            /*if (bid > register.MaxBid) 
                throw new BidOutRangeChallengeBidException("Bid must be lowwer of Challenge maximum bid");

            if (bid < register.MinBid)
                throw new BidOutRangeChallengeBidException("Bid must be bigger or equals of Challenge minimum bid");*/

            var user = await _dbContext.Users.FindAsync(user_id);

            if (user is null) throw new UserNotExistsException();

            if (user.Amount < bid) throw new BidOutRangeUserAmountException();

            var entity = new Bet(Guid.NewGuid(), challenge_id, user_id, bid);

            await _dbContext.Bets.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return OutputModelBet.BetToOutputBet(entity);
        }

        public async Task DeleteBet(Guid user_id, Guid bet_id)
        {
            var register = await _dbContext.Bets.Include(b => b.Challenge).FirstOrDefaultAsync(b => b.Id == bet_id);

            if (register is null) throw new BetNotExistsException();
            if (register.Challenge.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            var user = await _dbContext.Users.FindAsync(user_id);

            if (user is null) throw new UserNotExistsException();
            if (user.Id != register.IdBettor) throw new BetNotBelongsToUserException();

            _dbContext.Bets.Remove(register);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OutputModelBet> SelectBet(Guid bet_id)
        {
            var register = await _dbContext.Bets.FindAsync(bet_id);

            if (register is null) throw new BetNotExistsException();

            return OutputModelBet.BetToOutputBet(register);
        }

        public async Task<OutputModelChallenge> SelectChallenge(Guid bet_id)
        {
            var register = await _dbContext.Bets.Include(b => b.Challenge).FirstOrDefaultAsync(b => b.Id == bet_id);

            if (register is null) throw new BetNotExistsException();
            if (register.Challenge is null) throw new ChallengeNotExistsException();

            return OutputModelChallenge.ChallengeToOutputChallenge(register.Challenge);
        }

        public async Task UpdateBetBid(Guid user_id, Guid bet_id, decimal bid)
        {
            var register = await _dbContext.Bets.Include(b => b.Challenge).FirstOrDefaultAsync(b => b.Id == bet_id);

            if (register is null) throw new BetNotExistsException();

            var user = await _dbContext.Users.FindAsync(user_id);

            if (user is null) throw new UserNotExistsException();

            if (user.Id != register.IdBettor) throw new BetNotBelongsToUserException();

            if (user.Amount + register.Bid < bid) throw new BidOutRangeUserAmountException();

            if (register.Challenge is null) 
                throw new ChallengeNotExistsException();

            if (register.Challenge.Status != ChallengeStatus.ACTIVE)
                throw new ChallengeIsNotAtiveException();

            if (register.Challenge.IdChallenged == user_id)
                throw new ChallengedEqualsBettorException();

            /*if (bid > register.Challenge.MaxBid)
                throw new BidOutRangeChallengeBidException("Bid must be lowwer of Challenge maximum bid");

            if (bid < register.Challenge.MinBid)
                throw new BidOutRangeChallengeBidException("Bid must be bigger or equals of Challenge minimum bid");*/

            

            user.Amount += register.Bid - bid;
            register.Bid = bid;
            register.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }
    }
}
