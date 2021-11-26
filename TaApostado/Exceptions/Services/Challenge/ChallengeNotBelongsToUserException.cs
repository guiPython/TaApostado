using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeNotBelongsToUserException : Exception
    {
        protected new const string Message = "Challenge not belogs a this user";
        public ChallengeNotBelongsToUserException(string message = Message) : base(message) { }
    }
}
