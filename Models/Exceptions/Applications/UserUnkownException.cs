using System;

namespace Scadenze.Models.Exceptions.Application
{
    public class UserUnknownException : Exception
    {
        public UserUnknownException() : base($"A known user is required for this operation")
        {
        }
    }
}