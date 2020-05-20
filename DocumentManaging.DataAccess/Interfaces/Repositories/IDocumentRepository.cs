using DocumentManaging.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.DataAccess.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentByIdAsync(string partitionKey, string id);
        Task<List<Document>> GetDocumentsAsync(string partitionKey, OrderParameters parameters);
        Task<bool> DeleteDocumentAsync(string partitionKey, string id);
        Task<Document> CreateDocumentAsync(Document document, string partitionKey);
    }
}
