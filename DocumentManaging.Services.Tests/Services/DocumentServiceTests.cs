using DocumentManaging.DataAccess.Interfaces.Repositories;
using DocumentManaging.DataAccess.Interfaces.Storages;
using DocumentManaging.DataAccess.Models;
using DocumentManaging.Services.Services;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentManaging.Services.Tests.Services
{
    public class DocumentServiceTests
    {
        private Mock<IBlobStorage> _blobStorageMock;
        private Mock<IDocumentRepository> _documentRepositoryMock;

        public DocumentServiceTests()
        {
            _blobStorageMock = new Mock<IBlobStorage>();
            _documentRepositoryMock = new Mock<IDocumentRepository>();
        }

        [Fact]
        public void SaveDocumentAsync_ValidData_ResultDocumentObject()
        {
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            var fileName = "test.pdf";
            var userId = "123";
            var size = 1234;
            var document = new Document(userId, fileName, size);

            _blobStorageMock.Setup(x => x.CreateContainerAsync(userId)).ReturnsAsync(new Uri("http://test.com"));
            _blobStorageMock.Setup(x => x.CreateBlobAsync(userId, "test", fileStream)).ReturnsAsync(new Uri("http://test.com"));

            _documentRepositoryMock.Setup(x => x.CreateDocumentAsync(It.IsAny<Document>(), userId)).ReturnsAsync(document);

            var service = new DocumentService(_blobStorageMock.Object, _documentRepositoryMock.Object);

            var result = service.SaveDocumentAsync(fileStream, fileName, size, userId).GetAwaiter().GetResult();

            Assert.IsType<Document>(result);
            Assert.Equal(document, result);
        }
    }
}
