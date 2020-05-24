using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UrlShortener.Application.Functions
{
    public class ListUrl
    {
        private readonly ILogger<ListUrl> _logger;

        public ListUrl(ILogger<ListUrl> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("ListUrl")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            
            return new OkObjectResult("dasdsad");
        }
    }
}
