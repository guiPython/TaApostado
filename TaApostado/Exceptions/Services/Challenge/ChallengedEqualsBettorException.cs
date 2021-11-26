using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengedEqualsBettorException : Exception
    {
        protected new const string Message = "Challenged can't bet in your challenge";
        public ChallengedEqualsBettorException(string message = Message) : base(message) { }
    }
}
