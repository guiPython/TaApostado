using System;
using System.Collections.Generic;
using TaApostado.Entities.Enums;

namespace TaApostado.Entities
{
    public class User
    {
        public Guid Id { get;  set; }
        public string Name { get;  set; }
        public string LastName { get;  set; }
        public string CPF { get;  set; }
        public string Email { get;  set; }
        public string Password { get;  set; }
        public decimal Amount { get;  set; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; set; }
        public UserStatus Status { get;  set; }
        public List<Challenge> Challenges { get;  set; }
        public List<Bet> Bets { get;  set; }
        protected User() { }
        public User(Guid id, string name, string lastName, string cpf, string email, string password)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            CPF = cpf;
            Email = email;
            Password = password;
            Amount = 1000;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Status = UserStatus.ACTIVE;
            Bets = new List<Bet>();
            Challenges = new List<Challenge>();
        }

        public User(Guid id, string name, string lastName, string cpf, string email, string password, decimal amount)
            : this(id, name, lastName, cpf, email, password)
        {
            Amount = amount;
        }

        public string GetFullName() => Name + " " + LastName;
        public void SetAsActive() => Status = UserStatus.ACTIVE;
        public void SetAsSuspended() => Status = UserStatus.SUSPENDED;
        public void SetAsDeactivated() => Status = UserStatus.DEACTIVATED;
    }
}
