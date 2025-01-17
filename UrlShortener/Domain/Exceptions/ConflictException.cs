﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Domain.Exceptions
{
    public class ConflictException : DomainException
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, Exception inner) : base(message, inner) { }

        public override IActionResult HttpResult => new ConflictObjectResult(Message);
    }
}