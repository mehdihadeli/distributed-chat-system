using System;

namespace Chat.Application
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message)
        {
        }
    }
}