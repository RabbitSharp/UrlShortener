using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Models;
using UrlShortener.Domain;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Infrastructure;
using ApplicationException = UrlShortener.Application.Exceptions.ApplicationException;

namespace UrlShortener.Application.Functions
{
    public static class UpdateUrl
    {
        [FunctionName("UpdateUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request for UpdateUrl.");

            try
            {
                var loc = IServiceLocator.Instance;
                loc.RegisterServices(req, log, context);

                var dto = await Parser.Parse<UrlRequest>(req);

                var urlService = new UrlService();
                var urlResult = await urlService.Update(dto.SourceUrl, dto.Tail, dto.Description);

                var result = new UrlResponse(req.GetHostPath(), urlResult.LongUrl, urlResult.RowKey, urlResult.Description);

                log.LogInformation("Short Url updated.");
                return new OkObjectResult(result);
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
    }
}
