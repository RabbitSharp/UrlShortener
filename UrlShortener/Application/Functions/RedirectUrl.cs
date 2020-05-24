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
using ApplicationException = UrlShortener.Application.Exceptions.ApplicationException;

namespace UrlShortener.Application.Functions
{
    public class RedirectUrl
    {
        private readonly UrlService _urlService;
        private readonly ILogger<RedirectUrl> _logger;

        public RedirectUrl(UrlService urlService, ILogger<RedirectUrl> logger)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("RedirectUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UrlRedirect/{shortUrl}")] HttpRequest req,
            string shortUrl)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request for Url '{shortUrl}'.");
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
            catch (DomainException de)
            {
                _logger.LogError(de, "An domain error encountered.");
                return de.HttpResult;
            }
            catch (ApplicationException ae)
            {
                _logger.LogError(ae, "An validation error encountered.");
                return ae.HttpResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error was encountered.");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
