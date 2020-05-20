using DocumentManaging.DataAccess.Interfaces.Storages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.DataAccess.Storages
{
    public class BlobStorage : IBlobStorage
    {
        private readonly CloudStorageAccount _storageAccount;

        public BlobStorage(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }
        public async Task<Uri> CreateBlobAsync(string containerName, string blobLocation, Stream file)
        {
            var container = GetContainerByName(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(blobLocation);
            await cloudBlockBlob.UploadFromStreamAsync(file).ConfigureAwait(false);
            return cloudBlockBlob.Uri;
        }

        public async Task<Uri> CreateContainerAsync(string containerName)
        {
            var cloudBlobContainer = GetContainerByName(containerName);
            await cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            return cloudBlobContainer?.Uri;
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobLocation)
        {
            var container = GetContainerByName(containerName);
            var blob = container.GetBlockBlobReference(blobLocation);
            return await blob.DeleteIfExistsAsync();
        }

        public Task<Stream> GetBlobContantByLocationAsync(string containerName, string blobLocation)
        {
            var container = GetContainerByName(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(blobLocation);
            return cloudBlockBlob.OpenReadAsync();
        }

        private CloudBlobContainer GetContainerByName(string containerName)
        {
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            return cloudBlobClient.GetContainerReference(containerName);
        }
    }
}
