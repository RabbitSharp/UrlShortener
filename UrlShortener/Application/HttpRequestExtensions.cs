using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Application
{
    public static class HttpRequestExtensions
    {
        public static string GetHostPath(this HttpRequest req)
        {
            var uri = new Uri(req.GetEncodedUrl());
            return $"{uri.Scheme}://{uri.Host}";
        }

        public static IActionResult BuildRedirectResult(this HttpRequest req, string url)
        {
            req.HttpContext.Response.Headers.Add("Location", url);
            return new RedirectResult(url);
        }
    }
}