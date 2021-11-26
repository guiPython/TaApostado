using System;

namespace TaApostado.Exceptions.Services
{
    public class UserAlreadyExistsException : Exception
    {
        protected new const string Message = "User already exists";
        public UserAlreadyExistsException(string message = Message) : base(message) { }
    }
}
