using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Domain.Models
{
    public class ClickStatistic : TableEntity
    {
        public ClickStatistic() { }

        public ClickStatistic(string tail)
        {
            PartitionKey = tail;
            RowKey = Guid.NewGuid().ToString();
            TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        public string TimeStamp { get; set; }
    }
}