using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }

        public override IActionResult HttpResult => new BadRequestObjectResult(Message);
    }
}