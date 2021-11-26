using System;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Services.Interfaces;
using TaApostado.Persistence;
using Microsoft.EntityFrameworkCore;
using TaApostado.Exceptions.Services;

namespace TaApostado.Services
{
    public class ServiceUser : IServiceUser
    {
        private readonly DBContext _dbContext;
        public ServiceUser(DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Delete(Guid id)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user is null) throw new UserNotExistsException();

            user.SetAsDeactivated();
            user.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<OutputModelUser> Find(Guid id)
        {
            var user = await _dbContext.Users.Include(u => u.Challenges).Include(u => u.Bets).SingleOrDefaultAsync(u => u.Id == id);

            if (user is null) throw new UserNotExistsException();

            var challenges = user.Challenges.ConvertAll<OutputModelChallenge>(OutputModelChallenge.ChallengeToOutputChallenge);
            var bets = user.Bets.ConvertAll<OutputModelBet>(OutputModelBet.BetToOutputBet);

            return new OutputModelUser(user, challenges, bets);
        }

        public async Task Update(Guid id, InputModelUser user)
        {
            var register = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (register is null) throw new UserNotExistsException();

            register.Name = user.name;
            register.LastName = user.lastName;
            register.CPF = user.cpf;
            register.Email = user.email;
            register.Password = user.password;
            register.Amount = register.Amount;
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string name, string lastName)
        {
            var register = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (register is null) throw new UserNotExistsException();

            register.Name = name;
            register.LastName = name;
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string password)
        {
            var register = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (register is null) throw new UserNotExistsException();

            register.Password = password;
            register.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }
    }
}
