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
using ApplicationException = UrlShortener.Application.Exceptions.ApplicationException;

namespace UrlShortener.Application.Functions
{
    public class UpdateUrl
    {
        private readonly UrlService _urlService;
        private readonly ILogger<UpdateUrl> _logger;

        public UpdateUrl(UrlService urlService, ILogger<UpdateUrl> logger)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("UpdateUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for UpdateUrl.");

            try
            {
                //var loc = IServiceLocator.Instance;
                //loc.RegisterServices(req, log, context);

                var dto = await Parser.Parse<UrlRequest>(req);

                //var urlService = new UrlService();
                var urlResult = await _urlService.Update(dto.SourceUrl, dto.Tail, dto.Description);

                var result = new UrlResponse(req.GetHostPath(), urlResult.LongUrl, urlResult.RowKey, urlResult.Description);

                _logger.LogInformation("Short Url updated.");
                return new OkObjectResult(result);
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
