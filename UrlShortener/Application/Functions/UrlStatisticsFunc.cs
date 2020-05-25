using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Application.Functions
{
    public class UrlStatisticsFunc
    {
        private readonly ILogger<UrlStatisticsFunc> _logger;

        public UrlStatisticsFunc(ILogger<UrlStatisticsFunc> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(UrlStatistics))]
        public async Task<IActionResult> UrlStatistics(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"HTTP trigger function processed a request for {nameof(UrlStatistics)}.");

            return await GlobalErrorHandler.HandleExceptionAsync(async () => await UrlStatisticsAction(req), _logger);
        }

        private async Task<IActionResult> UrlStatisticsAction(HttpRequest req)
        {
            return new OkObjectResult(null);
        }
    }
}
