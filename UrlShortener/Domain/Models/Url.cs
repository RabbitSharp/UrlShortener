using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using UrlShortener.Infrastructure;

namespace UrlShortener.Domain.Models
{
    public class Url : TableEntity
    {
        private readonly IServiceLocator _locator;

        public Url()
        {
            
        }

        public Url(string longUrl, string endUrl) 
            : this(longUrl, endUrl, string.Empty)
        { }

        public Url(string longUrl, string endUrl, string desc)
        {
            _locator = IServiceLocator.GetInstance;

            PartitionKey = endUrl.First().ToString();
            RowKey = endUrl;
            SourceUrl = longUrl;
            Description = desc;
            ClickCount = 0;
        }

        //public string Id { get; set; }
        public string SourceUrl { get; set; }
        public string ShortUrl { get; set; } // => RowKey;
        public string Description { get; set; }
        public int ClickCount { get; set; }

        public override string ToString()
        {
            return $"SourceUrl: {SourceUrl} / ShortUrl: {ShortUrl} / Description: {Description} / ClickCount: {ClickCount}";
        }
    }
}