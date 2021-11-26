using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeQuotaIsInvalidException : Exception
    {
        protected new const string Message = "Invalid challenge quota";
        public ChallengeQuotaIsInvalidException(string message = Message) : base(message) { }
    }
}

