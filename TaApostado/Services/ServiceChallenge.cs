using System;
using System.Linq;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Persistence;
using TaApostado.Entities;
using TaApostado.Entities.Enums;
using TaApostado.Exceptions.Services;
using Microsoft.EntityFrameworkCore;

namespace TaApostado.Services
{
    public class ServiceChallenge : IServiceChallenge
    {
        private readonly DBContext _dbContext;
        public ServiceChallenge(DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OutputModelChallenge> CreateChallenge(Guid user_id, InputModelChallenge challenge)
        {
            var user = await _dbContext.Users.Include(u => u.Challenges).FirstOrDefaultAsync(u => u.Id == user_id);

            if (user is null) throw new UserNotExistsException();

            var challenges = user.Challenges.Where(c => c.Status == ChallengeStatus.ACTIVE || c.Status == ChallengeStatus.EXECUTION || c.Status == ChallengeStatus.VOTATION);

            if (challenges.Count() > 0) 
                throw new UserAlreadyParticipesTheChallengeException();

            if (challenge.quota < 1 || challenge.quota > challenge.bid)
                throw new ChallengeQuotaIsInvalidException();

            if ((challenge.bid % challenge.quota != 0) || (challenge.bid > user.Amount)) 
                throw new ChallengeBidIsInvalidException();

            var entity = new Challenge(Guid.NewGuid(), challenge.name, challenge.theme, challenge.bid, challenge.description, user_id, challenge.quota, challenge.timeOpen, challenge.timeExecution);
            await _dbContext.Challenges.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return OutputModelChallenge.ChallengeToOutputChallenge(entity);
        }

        public async Task<OutputModelChallenge> SelectChallenge(Guid challenge_id)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            return OutputModelChallenge.ChallengeToOutputChallenge(register);
        }

        public async Task UpdateChallenge(Guid challenge_id, Guid user_id, InputModelChallenge challenge)
        {

            var user = await _dbContext.Users.Include(u => u.Challenges).FirstOrDefaultAsync(u => u.Id == user_id);

            if (user is null) throw new UserNotExistsException();

            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();          

            if (challenge.quota > challenge.bid || challenge.quota < 1) throw new ChallengeQuotaIsInvalidException();

            if ((challenge.bid % challenge.quota != 0) || (challenge.bid > user.Amount)) throw new ChallengeBidIsInvalidException();

            register.Name = challenge.name;
            register.Theme = challenge.theme;
            register.ChangeBid(challenge.bid);
            register.ChangeQuota(challenge.quota);
            register.Description = challenge.description;
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeDescription(Guid challenge_id, Guid user_id, string description)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            if (register.Status == ChallengeStatus.SUSPENDED) throw new ChallengeIsSuspendedException();          

            register.ChangeDescription(description);
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeQuota(Guid challenge_id, Guid user_id, int quota)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) 
                throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) 
                throw new ChallengeIsNotAtiveException();

            if (register.Bid % quota != 0 || quota < 1 || quota > register.Bid)
                throw new ChallengeQuotaIsInvalidException();

            var oldQuota = register.Quota;

            register.ChangeQuota(quota);

            if (oldQuota < quota) register.Bets = register.Bets.GetRange(0, register.MaxBets);

            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeName(Guid challenge_id, Guid user_id, string name)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            if (register.Status == ChallengeStatus.SUSPENDED) throw new ChallengeIsSuspendedException();


            register.ChangeName(name);
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeTheme(Guid challenge_id, Guid user_id, string theme)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            if (register.Status == ChallengeStatus.SUSPENDED) throw new ChallengeIsSuspendedException();     

            register.ChangeTheme(theme);
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeBid(Guid challenge_id, Guid user_id, int bid)
        {
            var user = await _dbContext.Users.Include(u => u.Challenges).SingleOrDefaultAsync(u => u.Id == user_id);

            if (user is null) throw new UserNotExistsException();

            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (bid > user.Amount) 
                throw new ChallengeBidIsInvalidException();

            register.RemoveAllBets();
            register.ChangeBid(bid);
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateChallengeToExecution(Guid challenge_id, Guid user_id)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            register.SetAsExecution();
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateChallengeToVotation(Guid challenge_id, Guid user_id)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.EXECUTION) throw new ChallengeIsNotExecutionException();

            register.SetAsVotation();
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateChallengeToSuspended(Guid challenge_id, Guid user_id)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            register.SetAsSuspended();
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeToDeleted(Guid challenge_id, Guid user_id)
        {
            var register = await _dbContext.Challenges.FindAsync(challenge_id);

            if (register is null) throw new ChallengeNotExistsException();

            if (register.IdChallenged.Equals(user_id) is false) throw new ChallengeNotBelongsToUserException();

            if (register.Status != ChallengeStatus.ACTIVE) throw new ChallengeIsNotAtiveException();

            _dbContext.Remove(register);

            await _dbContext.SaveChangesAsync();
        }
    }
}
