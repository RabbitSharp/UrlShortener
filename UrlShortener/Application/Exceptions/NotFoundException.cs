using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
        public override IActionResult HttpResult => new NotFoundResult();
    }
}