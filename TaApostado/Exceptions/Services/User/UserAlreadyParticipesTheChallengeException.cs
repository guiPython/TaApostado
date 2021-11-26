using System;
namespace TaApostado.Exceptions.Services
{
    public class UserAlreadyParticipesTheChallengeException : Exception
    {
        protected new const string Message = "User already participe the challenge";
        public UserAlreadyParticipesTheChallengeException(string message = Message) : base(message) { }
    }
}