using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UrlShortener.Application.Exceptions;

namespace UrlShortener.Application
{
    public static class Parser
    {
        public static async Task<T> Parse<T>(HttpRequest req) 
            where T : class
        {
            if (req == null)
            {
                throw new NotFoundException();
            }
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var dto = JsonConvert.DeserializeObject<T>(requestBody);
            return dto ?? throw new NotFoundException();
        }
    }
}