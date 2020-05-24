using System;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using UrlShortener.Infrastructure;

namespace UrlShortener.Domain.Repositories
{
    public class ClickStatisticRepository
    {
        public const string TableName = "ClickStatistics";
        private readonly StorageTableHelper _storageTableHelper;

        public ClickStatisticRepository(StorageTableHelper storageTableHelper)
        {
            _storageTableHelper = storageTableHelper ?? throw new ArgumentNullException(nameof(storageTableHelper));
        }

        public async Task<ClickStatistic> Save(ClickStatistic entity)
        {
            var table = await _storageTableHelper.GetTable(TableName);
            return await _storageTableHelper.InsertOrUpdateAsync(table, entity);
        }
    }
}