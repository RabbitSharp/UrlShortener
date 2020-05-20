using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrlShortener.Application.Models;
using UrlShortener.Domain;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Infrastructure;

namespace UrlShortener.Application.Functions
{
    public static class AddUrl
    {
        [FunctionName("AddUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. {req.Method}");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var dto = JsonConvert.DeserializeObject<AddUrlRequest>(requestBody);
            if (dto == null)
            {
                return new NotFoundResult();
            }

            try
            {
                var loc = IServiceLocator.Instance;
                loc.RegisterServices(req, log, context);

                var urlService = new UrlService();
                var urlResult = await urlService.Add(dto.SourceUrl, dto.Tail, dto.Description);

                var uri = new Uri(req.GetEncodedUrl());
                var host = $"{uri.Scheme}://{uri.Host}";

                var result = new AddUrlResponse(host, urlResult.SourceUrl, urlResult.RowKey, urlResult.Description);

                log.LogInformation("Short Url created.");
                return new OkObjectResult(result);
            }
            catch (ConflictException de)
            {
                log.LogError(de, "An domain error encountered.");
                return de.HttpResult;
            }
            catch (Exception e)
            {
                log.LogError(e, "An unexpected error was encountered.");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
