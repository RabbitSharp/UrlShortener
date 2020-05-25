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
    public class ListUrlFunc
    {
        private readonly ILogger<ListUrlFunc> _logger;
        private readonly UrlService _urlService;

        public ListUrlFunc(ILogger<ListUrlFunc> logger, UrlService urlService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        }

        [FunctionName(nameof(ListUrl))]
        public async Task<IActionResult> ListUrl(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"HTTP trigger function processed a request for {nameof(ListUrl)}.");

            return await GlobalErrorHandler.HandleExceptionAsync(async () => await ListUrlAction(req), _logger);
        }

        private async Task<IActionResult> ListUrlAction(HttpRequest req)
        {
            var urls = await _urlService.GetAll();
            var result = new UrlListResponse(req.GetHostPath(), urls);
            return new OkObjectResult(result);
        }
    }
}
