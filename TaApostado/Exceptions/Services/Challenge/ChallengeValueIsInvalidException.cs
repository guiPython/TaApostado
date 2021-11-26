using System;

namespace TaApostado.Exceptions.Services
{
    public class ChallengeBidIsInvalidException : Exception
    {
        protected new const string Message = "Invalid challenge bid";
        public ChallengeBidIsInvalidException(string message = Message) : base(message) { }
    }
}
