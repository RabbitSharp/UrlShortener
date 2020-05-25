using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain;
using UrlShortener.Domain.Exceptions;

namespace UrlShortener.Application.Functions
{
    public class RedirectUrlFunc
    {
        private readonly UrlService _urlService;
        private readonly ILogger<RedirectUrlFunc> _logger;

        public RedirectUrlFunc(UrlService urlService, ILogger<RedirectUrlFunc> logger)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(RedirectUrl))]
        public async Task<IActionResult> RedirectUrl(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UrlRedirect/{shortUrl}")] HttpRequest req,
            string shortUrl)
        {
            _logger.LogInformation($"HTTP trigger function processed a request for {nameof(RedirectUrl)} with parameter '{shortUrl}'.");

            return await GlobalErrorHandler.HandleExceptionAsync(async () => await RedirectUrlAction(req, shortUrl), _logger);
        }

        private async Task<IActionResult> RedirectUrlAction(HttpRequest req, string shortUrl)
        {
            var defaultRedirectUrl = $"{req.GetHostPath()}/";

            try
            {
                if (string.IsNullOrWhiteSpace(shortUrl))
                {
                    _logger.LogInformation("Bad Link, using fallback.");
                    req.HttpContext.Response.Headers.Add("Location", defaultRedirectUrl);
                    return req.BuildRedirectResult(defaultRedirectUrl);
                }

                var entity = await _urlService.Get(shortUrl);

                var redirectUrl = WebUtility.UrlDecode(entity.LongUrl);
                return req.BuildRedirectResult(redirectUrl);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("Entity not found. Redirect to default.");
                return req.BuildRedirectResult(defaultRedirectUrl);
            }
        }
    }
}
