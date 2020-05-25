using UrlShortener.Domain.Models;

namespace UrlShortener.Application.Models
{
    public class UrlResponse
    {
        public UrlResponse() { }
        public UrlResponse(string host, string longUrl, string endUrl, string desc)
        {
            LongUrl = longUrl;
            ShortUrl = string.Concat(host, "/", endUrl);
            Description = desc;
        }

        public UrlResponse(string host, Url entity)
        {
            LongUrl = entity.LongUrl;
            ShortUrl = BuildShortUrl(host, entity.RowKey);
            Description = entity.Description;
        }

        public string ShortUrl { get; set; }
        public string LongUrl { get; set; }
        public string Description { get; set; }

        public static string BuildShortUrl(string host, string tail)
        {
            return string.Concat(host, "/", tail);
        }
    }
}