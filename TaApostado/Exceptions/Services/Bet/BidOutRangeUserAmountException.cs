using System;

namespace TaApostado.Exceptions.Services
{
    public class BidOutRangeUserAmountException : Exception
    {
        protected new const string Message = "Bid must be lowwer or equals User amount";
        public BidOutRangeUserAmountException(string message = Message): base(message) { }
    }
}
