using TaApostado.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TaApostado.DTOs.OutputModels
{
    public class OutputModelChallenge
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int bid { get; set; }
        public int quota { get; set; }
        public decimal maxBets { get; set; }
        public string status { get; set; }
        public DateTime ClosingBets { get; set; }
        public DateTime ExecutionChallenge {get; set;}
        public List<OutputModelBet> bets { get; set; }
        public static OutputModelChallenge ChallengeToOutputChallenge(Challenge c)
        {
            return new OutputModelChallenge()
            {
                id = c.Id.ToString(),
                name = c.Name,
                description = c.Description,
                bid = c.Bid,
                quota = c.Quota,
                maxBets = c.MaxBets,
                ClosingBets = c.CreatedAt.AddMinutes(c.TimeOpen),
                ExecutionChallenge = c.CreatedAt.AddMinutes(c.TimeOpen + c.TimeExecution),
                status = c.Status.ToString(),
                bets = c.Bets.Select(b => OutputModelBet.BetToOutputBet(b)).ToList()
            };
        }
    }
}
