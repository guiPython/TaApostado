using TaApostado.Entities;

namespace TaApostado.DTOs.OutputModels
{
    public class OutputModelBet
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public decimal bid { get; set; }

        public static OutputModelBet BetToOutputBet(Bet b)
        {
            return new OutputModelBet()
            {
                id = b.Id.ToString(),
                name = b.Bettor.Name + " " + b.Bettor.LastName,
                email = b.Bettor.Email,
                bid = b.Bid
            };
        }
    }
}
