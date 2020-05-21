using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException() { }
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception inner) : base(message, inner) { }

        public abstract IActionResult HttpResult { get; }
    }
}