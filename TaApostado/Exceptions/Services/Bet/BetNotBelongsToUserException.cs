using System;

namespace TaApostado.Exceptions.Services
{
    public class BetNotBelongsToUserException : Exception
    {
        protected new const string Message = "Challenge not belongs to user";
        public BetNotBelongsToUserException(string message = Message) : base(message) { }
    }
}
