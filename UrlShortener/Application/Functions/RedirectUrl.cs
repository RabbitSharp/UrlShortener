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
using UrlShortener.Infrastructure;
using ApplicationException = UrlShortener.Application.Exceptions.ApplicationException;

namespace UrlShortener.Application.Functions
{
    public static class RedirectUrl
    {
        [FunctionName("RedirectUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UrlRedirect/{shortUrl}")]
            HttpRequest req,
            string shortUrl,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# HTTP trigger function processed a request for Url '{shortUrl}'.");
            var defaultRedirectUrl = $"{req.GetHostPath()}/";

            try
            {
                var loc = IServiceLocator.Instance;
                loc.RegisterServices(req, log, context);

                if (string.IsNullOrWhiteSpace(shortUrl))
                {
                    log.LogInformation("Bad Link, using fallback.");
                    req.HttpContext.Response.Headers.Add("Location", defaultRedirectUrl);
                    return req.BuildRedirectResult(defaultRedirectUrl);
                }

                var urlService = new UrlService();
                var entity = await urlService.Get(shortUrl);

                var redirectUrl = WebUtility.UrlDecode(entity.LongUrl);
                return req.BuildRedirectResult(redirectUrl);
            }
            catch (EntityNotFoundException)
            {
                log.LogWarning("Entity not found. Redirect to default.");
                return req.BuildRedirectResult(defaultRedirectUrl);
            }
            catch (DomainException de)
            {
                log.LogError(de, "An domain error encountered.");
                return de.HttpResult;
            }
            catch (ApplicationException ae)
            {
                log.LogError(ae, "An validation error encountered.");
                return ae.HttpResult;
            }
            catch (Exception e)
            {
                log.LogError(e, "An unexpected error was encountered.");
                return new BadRequestObjectResult(e.Message);
            }
        }

        private static IActionResult BuildRedirectResult(string url, HttpRequest req)
        {
            req.HttpContext.Response.Headers.Add("Location", url);
            return new RedirectResult(url);
        }
    }
}
