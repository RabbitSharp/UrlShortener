using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UrlShortener.Application.Models;
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

            var url = dto.SourceUrl.Trim();
            var tail = dto.Tail.Trim();
            var desc = dto.Description.Trim();


            var loc = IServiceLocator.GetInstance;
            loc.RegisterExternalServices(req, log, context);

            var result = new AddUrlResponse();



            // Continue here
            // https://github.com/FBoucher/AzUrlShortener/blob/master/src/shortenerTools/Domain/StorageTableHelper.cs


            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name ??= data?.name;

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            return new OkObjectResult($"Hello, dddddd");
        }
    }
}
