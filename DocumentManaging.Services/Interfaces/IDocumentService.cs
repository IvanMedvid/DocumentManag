using DocumentManaging.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetDocumentsAsync(string userId, OrderParameters parameters);

        Task<Document> SaveDocumentAsync(Stream file, string fileName, int size, string userId);

        Task<Document> GetDocumentByIdAsync(string id, string userId);

        Task<Stream> LoadDocumentStreamAsync(string location, string userId);

        Task<bool> DeleteDocumentAsync(string id, string userId);
    }
}
