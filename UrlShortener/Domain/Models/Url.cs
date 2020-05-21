using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Domain.Models
{
    public class Url : TableEntity
    {

        public Url()
        {
        }

        public Url(string shortUrl) 
            : this(string.Empty, shortUrl, string.Empty)
        { }

        public Url(string longUrl, string shortUrl, string desc)
        {
            PartitionKey = shortUrl.First().ToString();
            RowKey = shortUrl;
            LongUrl = longUrl;
            Description = desc;
            ClickCount = 0;
        }

        //public string Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl => RowKey;
        public string Description { get; set; }
        public int ClickCount { get; set; }

        public override string ToString()
        {
            return $"LongUrl: {LongUrl} / ShortUrl: {ShortUrl} / Description: {Description} / ClickCount: {ClickCount}";
        }
    }
}