using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UrlShortener.Domain.Models;

namespace UrlShortener.Infrastructure
{
    public class StorageTableHelper
    {
        private readonly Config _config;
        private readonly ILogger<StorageTableHelper> _logger;

        public StorageTableHelper(Config config, ILogger<StorageTableHelper> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            try
            {
                _logger.LogWarning(_config.StorageConnectionString);
                return CloudStorageAccount.Parse(_config.StorageConnectionString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Invalid storage account information provided.");
                throw;
            }
        }

        public async Task<CloudTable> GetTable(string tableName)
        {
            var storageAccount = CreateStorageAccountFromConnectionString();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task<List<T>> GetAllAsync<T>(CloudTable table)
            where T : ITableEntity, new()
        {
            TableContinuationToken token = null;
            var entities = new List<T>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<T>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public async Task<T> GetEntityAsync<T>(CloudTable table, string partitionKey, string rowKey) 
            where T : class, ITableEntity
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                var result = await table.ExecuteAsync(retrieveOperation);
                var entity = result.Result as T;
                if (entity != null)
                {
                    _logger.LogInformation("\tPartitionKey: {0}\tRowKey: {1}\tEntity: {2}", entity.PartitionKey, entity.RowKey, entity);
                }

                return entity;
            }
            catch (StorageException e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public Task<T> GetEntityAsync<T>(CloudTable table, T row)
            where T : class, ITableEntity
        {
            return GetEntityAsync<T>(table, row.PartitionKey, row.RowKey);
        }

        public async Task<T> InsertOrUpdateAsync<T>(CloudTable table, T addEntity)
            where T : class, ITableEntity
        {
            var insertOperation = TableOperation.InsertOrMerge(addEntity);
            var result = await table.ExecuteAsync(insertOperation);
            return result.Result as T;
        }

        public async Task DeleteEntityAsync<T>(CloudTable table, T deleteEntity)
            where T: class, ITableEntity
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException(nameof(deleteEntity));
                }

                var deleteOperation = TableOperation.Delete(deleteEntity);
                var result = await table.ExecuteAsync(deleteOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<int> GetNextTableId(CloudTable table)
        {
            // Get current Id
            var selectOperation = TableOperation.Retrieve<IdEntity>("1", "KEY");
            var result = await table.ExecuteAsync(selectOperation);

            if (!(result.Result is IdEntity entity))
            {
                entity = new IdEntity
                {
                    PartitionKey = "1",
                    RowKey = "KEY",
                    Id = 1024
                };
            }
            entity.Id++;

            // Update
            var updateOperation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(updateOperation);

            return entity.Id;
        }

    }
}