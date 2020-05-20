using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Domain.Models
{
    public class IdEntity : TableEntity
    {
        public int Id { get; set; }
    }
}