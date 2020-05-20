using DocumentManaging.DataAccess.Interfaces.Repositories;
using DocumentManaging.DataAccess.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.DataAccess.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly CosmosClient _cosmosDbClient;
        private const string DatabaseId = "document-managing";
        private const string ContainerId = "documents";
        public DocumentRepository(CosmosClient cosmosDbClient)
        {
            _cosmosDbClient = cosmosDbClient;
        }
 
        public async Task<Document> CreateDocumentAsync(Document document, string partitionKey)
        {
            var container = _cosmosDbClient.GetContainer(DatabaseId, ContainerId);

            var result = await container.CreateItemAsync(document, new PartitionKey(partitionKey));

            return result.Resource;
        }

        public async Task<bool> DeleteDocumentAsync(string partitionKey, string id)
        {
            var container = _cosmosDbClient.GetContainer(DatabaseId, ContainerId);

            var result = await container.DeleteItemAsync<Document>(id, new PartitionKey(partitionKey));

            return result.StatusCode == HttpStatusCode.NoContent ? true : false;
        }

        public async Task<Document> GetDocumentByIdAsync(string partitionKey, string id)
        {
            var container = _cosmosDbClient.GetContainer(DatabaseId, ContainerId);

            var result = await container.ReadItemAsync<Document>(id, new PartitionKey(partitionKey));

            return result.Resource;
        }

        public async Task<List<Document>> GetDocumentsAsync(string partitionKey, OrderParameters parameters)
        {
            var container = _cosmosDbClient.GetContainer(DatabaseId, ContainerId);

            var query = container.GetItemLinqQueryable<Document>(true)
                .Where(c => c.UserId == partitionKey);

            var result = await ExecuteQueryAsync(query);

            return result;
        }

        private async Task<List<TDocument>> ExecuteQueryAsync<TDocument>(IQueryable<TDocument> query)
        {
            var documents = new List<TDocument>();

            var feedIterator = query.ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                var nextResults = await feedIterator.ReadNextAsync();
                documents.AddRange(nextResults);
            }

            return documents;
        }
    }
    
}
