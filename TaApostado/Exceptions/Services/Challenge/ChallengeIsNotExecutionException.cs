using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeIsNotExecutionException : Exception
    {
        protected new const string Message = "Challeng isn't execution";
        public ChallengeIsNotExecutionException(string message = Message) : base(message) { }
    }
}
