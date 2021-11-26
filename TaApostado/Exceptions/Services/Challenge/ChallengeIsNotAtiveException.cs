using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeIsNotAtiveException : Exception
    {
        protected new const string Message = "Challenge isn't active";
        public ChallengeIsNotAtiveException(string message = Message) : base(message) { }
    }
}
