using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeIsSuspendedException : Exception
    {
        protected new const string Message = "Challenge is Suspended";
        public ChallengeIsSuspendedException(string message = Message) : base(message) { }
    }
}
