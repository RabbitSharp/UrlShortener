using System.Collections.Generic;
using UrlShortener.Domain.Models;

namespace UrlShortener.Application.Models
{
    public class UrlStatsListResponse
    {
        public UrlStatsListResponse() { }

        public UrlStatsListResponse(IEnumerable<ClickStatistic> list)
        {
            ClickStatsList = new List<UrlStatResponse>();
            foreach (var statistic in list)
            {
                ClickStatsList.Add(new UrlStatResponse(statistic));
            }
        }

        public List<UrlStatResponse> ClickStatsList { get; set; }
    }

    public class UrlStatResponse
    {
        public UrlStatResponse() { }

        public UrlStatResponse(ClickStatistic entity)
        {
            Tail = entity.RowKey;
            ClickDate = entity.TimeStamp;
        }

        public string Tail { get; set; }
        public string ClickDate { get; set; }
    }
}