using System.Collections.Generic;
using System.Text.Json.Serialization;
using TaApostado.Entities;

namespace TaApostado.DTOs.OutputModels
{
    public class OutputModelUser
    {
        public string name{ get; set;}
        public decimal amount { get; set; }
        public List<OutputModelChallenge> challenges { get; set; }
        public List<OutputModelBet> bets { get; set; }

        [JsonConstructorAttribute]
        public OutputModelUser(string name, decimal amount, List<OutputModelChallenge> challenges, List<OutputModelBet> bets)
        {
            this.name = name;
            this.amount = amount;
            this.challenges = challenges;
            this.bets = bets;
        }
            

        public OutputModelUser(User u, List<OutputModelChallenge> challenges, List<OutputModelBet> bets)
        {
            name = u.Name;
            amount = u.Amount;
            this.challenges = challenges;
            this.bets = bets;
        }

    }

    
}
