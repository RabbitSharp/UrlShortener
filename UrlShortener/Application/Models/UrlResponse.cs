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

        public string ShortUrl { get; set; }
        public string LongUrl { get; set; }
        public string Description { get; set; }
    }
}