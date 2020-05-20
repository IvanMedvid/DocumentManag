using DocumentManaging.DataAccess.Interfaces.Repositories;
using DocumentManaging.DataAccess.Interfaces.Storages;
using DocumentManaging.DataAccess.Models;
using DocumentManaging.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.Services.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IDocumentRepository _documentRepository;
        public DocumentService(IBlobStorage blobStorage, IDocumentRepository documentRepository)
        {
            _blobStorage = blobStorage;
            _documentRepository = documentRepository;
        }
        public async Task<bool> DeleteDocumentAsync(string id, string userId)
        {
            var isDocumentExist = await _documentRepository.DeleteDocumentAsync(userId, id);

            if (!isDocumentExist)
            {
                return false;
            }
   
            await _blobStorage.DeleteBlobAsync(userId, id);

            return true;
        }

        public async Task<Document> GetDocumentByIdAsync(string id, string userId)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(userId, id);
            
            return document;                       
        }

        public async Task<IEnumerable<Document>> GetDocumentsAsync(string userId, OrderParameters parameters)
        {
            var documents =  await _documentRepository.GetDocumentsAsync(userId, parameters);

            return documents;
        }

        public Task<Stream> LoadDocumentStreamAsync(string location, string userId)
        {
            return _blobStorage.GetBlobContantByLocationAsync(userId, location);

        }

        public async Task<Document> SaveDocumentAsync(Stream file, string fileName, int size, string userId)
        {
            var document = new Document(userId, fileName, size);

            await _blobStorage.CreateContainerAsync(userId);
            await _blobStorage.CreateBlobAsync(userId, document.Id, file);

            var result = await _documentRepository.CreateDocumentAsync(document, userId);

            return result;
        }
    }
}
