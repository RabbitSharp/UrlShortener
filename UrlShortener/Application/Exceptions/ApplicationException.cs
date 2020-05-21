using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Application.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        protected ApplicationException() { }
        protected ApplicationException(string message) : base(message) { }
        protected ApplicationException(string message, Exception inner) : base(message, inner) { }

        public abstract IActionResult HttpResult { get; }
    }
}