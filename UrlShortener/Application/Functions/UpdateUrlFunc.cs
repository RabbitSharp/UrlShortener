using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Models;
using UrlShortener.Domain;

namespace UrlShortener.Application.Functions
{
    public class UpdateUrlFunc
    {
        private readonly UrlService _urlService;
        private readonly ILogger<UpdateUrlFunc> _logger;

        public UpdateUrlFunc(UrlService urlService, ILogger<UpdateUrlFunc> logger)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(UpdateUrl))]
        public async Task<IActionResult> UpdateUrl(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"HTTP trigger function processed a request for {nameof(UpdateUrl)}.");

            return await GlobalErrorHandler.HandleExceptionAsync(async () => await UpdateUrlAction(req), _logger);
        }

        private async Task<IActionResult> UpdateUrlAction(HttpRequest req)
        {
            var dto = await Parser.Parse<UrlRequest>(req);

            var urlResult = await _urlService.Update(dto.SourceUrl, dto.Tail, dto.Description);

            var result = new UrlResponse(req.GetHostPath(), urlResult.LongUrl, urlResult.RowKey, urlResult.Description);

            _logger.LogInformation("Short Url updated.");
            return new OkObjectResult(result);
        }
    }
}
