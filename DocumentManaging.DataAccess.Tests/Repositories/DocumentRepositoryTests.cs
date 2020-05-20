using Microsoft.Azure.Cosmos;
using DocumentManaging.DataAccess.Models;
using Moq;
using System.Reflection.Metadata;
using System.Threading;
using Xunit;
using Document = DocumentManaging.DataAccess.Models.Document;
using DocumentManaging.DataAccess.Repositories;

namespace DocumentManaging.DataAccess.Tests.Repositories
{
    public class DocumentRepositoryTests
    {
        private Mock<CosmosClient> _cosmosDbClientMock;
        private Mock<Container> _containerMock;
        public DocumentRepositoryTests()
        {
            _cosmosDbClientMock = new Mock<CosmosClient>(MockBehavior.Strict);
            _containerMock = new Mock<Container>();
        }

        [Fact]
        public void GetDocumentByIdAsync_ValidData_RetunrsDocument()
        {
            var userId = "123";
            var documentId = "6ca501c1-67f4-4c07-9064-4c9f591899c3";
            var document = new Document(userId, "test.pdf", 123);

            var responseMock = new Mock<ItemResponse<Document>>();
            responseMock.Setup(p => p.Resource).Returns(document);

            _containerMock.Setup(x => x.ReadItemAsync<Document>(documentId, new PartitionKey(userId), null, default(CancellationToken))).ReturnsAsync(responseMock.Object);
            _cosmosDbClientMock.Setup(x => x.GetContainer("document-managing", "documents")).Returns(_containerMock.Object);

            var repository = new DocumentRepository(_cosmosDbClientMock.Object);

            var result = repository.GetDocumentByIdAsync(userId, documentId).GetAwaiter().GetResult();

            Assert.IsType<Document>(result);
            Assert.Equal(document, result);

        }
    }
}
