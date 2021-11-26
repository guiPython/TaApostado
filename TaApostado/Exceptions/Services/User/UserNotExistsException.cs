using System;

namespace TaApostado.Exceptions.Services
{
    public class UserNotExistsException : Exception
    {
        protected new const string Message = "User not exists";
        public UserNotExistsException(string message = Message) : base(message) { }
    }
}