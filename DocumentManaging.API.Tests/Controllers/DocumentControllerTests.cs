using DocumentManaging.API.Controllers;
using DocumentManaging.DataAccess.Models;
using DocumentManaging.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentManaging.API.Tests.Controllers
{
    public class DocumentControllerTests
    {
        private Mock<IDocumentService> _documentServiceMock;
        private Mock<ILogger<DocumentController>> _loggerMock;
        public DocumentControllerTests()
        {
            _documentServiceMock = new Mock<IDocumentService>();
            _loggerMock = new Mock<ILogger<DocumentController>>();
        }

        [Fact]
        public void Post_DataValid_FileSavedToBlobAndDocumentInfoReturns()
        {
            var userId = "123";
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.ContentType).Returns("application/pdf");
            fileMock.Setup(x => x.Length).Returns(1234);
            fileMock.Setup(x => x.FileName).Returns("test.pdf");
            fileMock.Setup(x => x.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

            _documentServiceMock.Setup(x => x.SaveDocumentAsync(fileMock.Object.OpenReadStream(), fileMock.Object.FileName, (int)fileMock.Object.Length, userId))
                .ReturnsAsync(new Document(userId, fileMock.Object.FileName, (int)fileMock.Object.Length));

            var documentController = new DocumentController(_documentServiceMock.Object, _loggerMock.Object);

            var result = documentController.Post(userId, fileMock.Object).GetAwaiter().GetResult();

            Assert.IsType<ActionResult<Document>>(result);
            Assert.IsType<OkObjectResult>(result.Result);

        }

        [Fact]
        public void Post_NotPdfFile_FileNotSavedBadRequestReturns()
        {
            var userId = "123";
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.ContentType).Returns("application/xml");
            fileMock.Setup(x => x.Length).Returns(1234);
            fileMock.Setup(x => x.FileName).Returns("test.xml");

            var documentController = new DocumentController(_documentServiceMock.Object, _loggerMock.Object);

            var result = documentController.Post(userId, fileMock.Object).GetAwaiter().GetResult();

            Assert.IsType<ActionResult<Document>>(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

        }
    }
}
