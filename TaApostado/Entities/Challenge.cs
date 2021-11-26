using System;
using TaApostado.Entities.Enums;
using System.Collections.Generic;

namespace TaApostado.Entities
{
    public class Challenge
    {
        public Guid Id { get;  set; }
        public Guid IdChallenged { get;  set; }
        public string Name { get;  set; }
        public string Theme { get;  set; }
        public string Description { get;  set; }
        public int Bid { get; set; } 
        public int Quota { get; set; }
        public int MaxBets { get; set; }
        public int TimeOpen { get; set; }
        public int TimeExecution { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }
        public List<Bet> Bets { get; set; }
        public User Challenged { get;  set; }
        public ChallengeStatus Status { get;  set; }
        public Challenge(Guid id, string name, string theme, int bid, string description, Guid idChallenged, int quota, int timeOpen, int timeExecution)
        {
            Id = id;
            Name = name;
            Theme = theme;
            Description = description;
            IdChallenged = idChallenged;
            Status = ChallengeStatus.ACTIVE;
            Bid = bid;
            Quota = quota;
            TimeOpen = timeOpen;
            TimeExecution = timeExecution;
            MaxBets = bid / quota;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Bets = new List<Bet>();
        }

        public void ChangeName(string name) => Name = name;
        public void ChangeBid(int bid)
        {
            Bid = bid;
            MaxBets = Bid / Quota;
        }
        public void ChangeQuota(int quota)
        {
            Quota = quota;
            MaxBets = Bid / Quota;
        }
        public void ChangeTheme(string theme) => Theme = theme;
        public void ChangeDescription(string description) => Description = description;
        public static int GetTimeVotation() => 2;
        public void RemoveAllBets() => Bets = new List<Bet>();
        public void SetAsExecution() => Status = ChallengeStatus.EXECUTION;
        public void SetAsVotation() => Status = ChallengeStatus.VOTATION;
        public void SetAsSuspended() => Status = ChallengeStatus.SUSPENDED;
    }
}
