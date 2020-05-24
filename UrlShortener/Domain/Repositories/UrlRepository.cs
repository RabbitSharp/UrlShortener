using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using UrlShortener.Infrastructure;

namespace UrlShortener.Domain.Repositories
{
    public class UrlRepository
    {
        public const string TableName = "Urls";
        private readonly StorageTableHelper _storageTableHelper;

        public UrlRepository(StorageTableHelper storageTableHelper)
        {
            _storageTableHelper = storageTableHelper ?? throw new ArgumentNullException(nameof(storageTableHelper));
        }

        public async Task<Url> GetEntity(Url entity)
        {
            var table = await _storageTableHelper.GetTable(TableName);
            return await _storageTableHelper.GetEntityAsync<Url>(table, entity.PartitionKey, entity.RowKey);
        }

        public async Task<List<Url>> GetAll()
        {
            var table = await _storageTableHelper.GetTable(TableName);
            return await _storageTableHelper.GetAllAsync<Url>(table);
        }

        public async Task<Url> Save(Url entity)
        {
            var table = await _storageTableHelper.GetTable(TableName);
            return await _storageTableHelper.InsertOrUpdateAsync(table, entity);
        }

        public async Task<bool> ExistAsync(Url obj)
        {
            var table = await _storageTableHelper.GetTable(TableName);
            var entity = await _storageTableHelper.GetEntityAsync(table, obj);
            return entity != null;
        }

        public async Task<int> GetNextTableId()
        {
            var table = await _storageTableHelper.GetTable(TableName);
            return await _storageTableHelper.GetNextTableId(table);

        }
    }
}