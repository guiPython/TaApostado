using System;
using TaApostado.Entities.Enums;

namespace TaApostado.Entities
{
    public class Bet
    {
        public Guid Id { get;  set; }
        public Guid IdChallenge { get;  set; }
        public Guid IdBettor { get;  set; }
        public decimal Bid { get;  set; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; set; }
        public Vote Vote { get; set; }
        public Challenge Challenge { get;  set; }
        public User Bettor { get;  set; }
        public Bet(Guid id, Guid idChallenge, Guid idBettor, decimal bid)
        {
            Id = id;
            IdChallenge = idChallenge;
            IdBettor = idBettor;
            Bid = bid;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Vote = Vote.None;
        }
    }
}
