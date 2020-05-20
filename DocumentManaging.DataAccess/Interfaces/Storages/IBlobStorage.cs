using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManaging.DataAccess.Interfaces.Storages
{
    public interface IBlobStorage
    {
        Task<Uri> CreateContainerAsync(string containerName);
        Task<Uri> CreateBlobAsync(string containerName, string blobLocation, Stream file);
        Task<Stream> GetBlobContantByLocationAsync(string containerName, string blobLocation);
        Task<bool> DeleteBlobAsync(string containerName, string blobLocation);
    }
}
