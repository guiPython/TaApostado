using System;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;

namespace TaApostado.Services.Interfaces
{
    public interface IServiceChallenge
    {
        public Task<OutputModelChallenge> CreateChallenge(Guid user_id, InputModelChallenge challenge);
        public Task<OutputModelChallenge> SelectChallenge(Guid challenge_id);
        public Task UpdateChallenge(Guid challenge_id, Guid user_id, InputModelChallenge challenge);
        public Task UpdateChallengeName(Guid challenge_id, Guid user_id, string name);
        public Task UpdateChallengeTheme(Guid id, Guid user_id, string theme); 
        public Task UpdateChallengeQuota(Guid challenge_id, Guid user_id, int quota);
        public Task UpdateChallengeBid(Guid challenge_id, Guid user_id, int bid);
        public Task UpdateChallengeDescription(Guid challenge_id, Guid user_id, string description);
        public Task UpdateChallengeToExecution(Guid challenge_id, Guid user_id);
        public Task UpdateChallengeToVotation(Guid challenge_id, Guid user_id);
        public Task UpdateChallengeToSuspended(Guid challenge_id, Guid user_id);
        public Task UpdateChallengeToDeleted(Guid challenge_id, Guid user_id);
    }
}
