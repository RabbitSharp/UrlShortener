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
    public class UrlStatisticsFunc
    {
        private readonly ILogger<UrlStatisticsFunc> _logger;
        private readonly StatisticService _statService;

        public UrlStatisticsFunc(ILogger<UrlStatisticsFunc> logger, StatisticService statService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _statService = statService ?? throw new ArgumentNullException(nameof(statService));
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
            var dto = await Parser.Parse<UrlStatsRequest>(req);
            var stats = await _statService.GetAllByTail(dto.Tail);

            var result = new UrlStatsListResponse(stats);
            return new OkObjectResult(result);
        }
    }
}
