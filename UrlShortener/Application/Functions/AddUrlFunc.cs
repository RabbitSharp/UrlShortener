using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Models;
using UrlShortener.Domain;

namespace UrlShortener.Application.Functions
{
    public class AddUrlFunc
    {
        private readonly UrlService _urlService;
        private readonly ILogger<AddUrlFunc> _logger;

        public AddUrlFunc(UrlService urlService, ILogger<AddUrlFunc> logger)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(AddUrl))]
        public async Task<IActionResult> AddUrl(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"HTTP trigger function processed a request for {nameof(AddUrl)}.");

            return await GlobalErrorHandler.HandleExceptionAsync(async () => await AddUrlAction(req), _logger);
        }

        private async Task<IActionResult> AddUrlAction(HttpRequest req)
        {
            var dto = await Parser.Parse<UrlRequest>(req);

            var urlResult = await _urlService.Add(dto.SourceUrl, dto.Tail, dto.Description);
            var result = new UrlResponse(req.GetHostPath(), urlResult.LongUrl, urlResult.RowKey, urlResult.Description);

            _logger.LogInformation("Short Url created.");
            return new OkObjectResult(result);
        }
    }
}
