using System;

namespace TaApostado.Exceptions.Services
{
    public class BidOutRangeChallengeBidException : Exception
    {
        public BidOutRangeChallengeBidException(string message) : base(message) { }
    }
}
