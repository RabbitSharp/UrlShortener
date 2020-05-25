using System.Collections.Generic;
using System.Linq;
using UrlShortener.Domain.Models;

namespace UrlShortener.Application.Models
{
    public class UrlListResponse
    {
        public UrlListResponse() { }
        public UrlListResponse(IEnumerable<UrlResponse> list)
        {
            UrlList = list.ToList();
        }
        public UrlListResponse(string host, IEnumerable<Url> list)
        {
            UrlList = new List<UrlResponse>();
            foreach (var url in list)
            {
                UrlList.Add(new UrlResponse(host, url));
            }
        }

        public List<UrlResponse> UrlList { get; set; }
    }
}