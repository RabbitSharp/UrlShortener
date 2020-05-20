namespace UrlShortener.Application.Models
{
    public class AddUrlRequest
    {
        public string SourceUrl { get; set; }
        public string Tail { get; set; }
        public string Description { get; set; }
    }
}