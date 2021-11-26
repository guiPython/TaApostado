using System;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using TaApostado.Entities;
using TaApostado.Services.Interfaces;
using TaApostado.Persistence;
using TaApostado.Utils;
using Microsoft.EntityFrameworkCore;
using TaApostado.Exceptions.Services;
using System.Security.Cryptography;
using System.Linq;

namespace TaApostado.Services
{
    public class ServiceLogin : IServiceLogin
    {
        private readonly DBContext _dbContext;
        public ServiceLogin(DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OutputModelLogin> SignIn(InputModelLogin login)
        {
            var user = await _dbContext.Users.Include(u => u.Challenges).Include(u => u.Bets).SingleOrDefaultAsync(u => u.Email.Equals(login.email) && MD5.Equals(u.Password, MD5Utils.Generate(login.password)));
            if (user is null) throw new UserNotExistsException();

            Console.WriteLine(user.Challenges);
            var challenges = user.Challenges.Select(c => OutputModelChallenge.ChallengeToOutputChallenge(c)).ToList();
            var bets = user.Bets.Select(b => OutputModelBet.BetToOutputBet(b)).ToList();

            return new OutputModelLogin(new OutputModelUser(user, challenges, bets), TokenUtils.Generate(user));
        }

        public async Task<OutputModelLogin> SignUp(InputModelUser user)
        {
            var register = await _dbContext.Users.SingleOrDefaultAsync(u => u.CPF == MD5Utils.Generate(user.cpf) || u.Email == user.email);
            if (register is not null) throw new UserAlreadyExistsException();

            var entity = new User(Guid.NewGuid(), user.name, user.lastName, MD5Utils.Generate(user.cpf), user.email, MD5Utils.Generate(user.password));
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            var challenges = entity.Challenges.Select(c => OutputModelChallenge.ChallengeToOutputChallenge(c)).ToList();
            var bets = entity.Bets.Select(b => OutputModelBet.BetToOutputBet(b)).ToList();

            return new OutputModelLogin(new OutputModelUser(entity, challenges, bets), TokenUtils.Generate(entity));
        }
    }
}
