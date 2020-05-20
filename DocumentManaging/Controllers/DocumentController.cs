using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentManaging.DataAccess.Models;
using DocumentManaging.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;

namespace DocumentManaging.API.Controllers
{
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private const int FileSizeLimit = 52428800; //5Mb

        public DocumentController(
            IDocumentService documentService,
            ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>Gets list of documents by the specified order parameters. Default value name.asc.</summary>
        /// <param name="userId">Id of user that performing operation.</param>
        /// <param name="parameters">Ordering parameters.</param>
        /// <returns>List of documents.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> Get([FromRoute]string userId)
        {
            var policy = Policy<IEnumerable<Document>>
                .Handle<Exception>()
                .FallbackAsync((IEnumerable<Document>)null);

            var documents = await policy
                .ExecuteAsync(async () => await _documentService.GetDocumentsAsync(userId, new OrderParameters()))
                .ConfigureAwait(false);

            if(documents != null)
            {
                return Ok(documents);
            }

            return StatusCode(500);
        }

        /// <summary>Downloads the document by specified document id.</summary>
        /// <param name="userId">Id of user that performing operation.</param>
        /// <param name="id">The document id.</param>
        /// <returns>File content.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> Download([FromRoute]string userId, string id)
        {
            var documentPolicy = Policy<Document>
                .Handle<Exception>()
                .FallbackAsync((Document)null);

            var streamPolicy = Policy<Stream>
                .Handle<Exception>()
                .FallbackAsync((Stream)null);

            var document = await documentPolicy
                .ExecuteAsync(async () => await _documentService.GetDocumentByIdAsync(id, userId))
                .ConfigureAwait(false);
            
            if (document == null)
            {
                var message = $"Document info by id: {id} does not exist.";
                _logger.LogTrace(message);

                return NotFound(message);
            }

            var stream = await streamPolicy
                .ExecuteAsync(async () => await _documentService.LoadDocumentStreamAsync(id, userId))
                .ConfigureAwait(false);
            
            if (stream == null)
            {
                var message = $"Document data by by id: {id} does not exist.";
                _logger.LogTrace(message);

                return NotFound();
            }

            return File(stream, "application/octet-stream", document.Name);
        }

        /// <summary>
        /// Save the specified file.
        /// </summary>
        /// <param name="userId">Id of user that performing operation.</param>
        /// <param name="file">The file.</param>
        /// <returns>Saved document info.</returns>
        [HttpPost]
        public async Task<ActionResult<Document>> Post([FromRoute]string userId, IFormFile file)
        {
            
            if (!file.ContentType.Equals("application/pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Only pdf files allowed.");
            }

            if (file.Length > FileSizeLimit)
            {
                return BadRequest("File size more then 5Mb.");
            }

            var documentPolicy = Policy<Document>
               .Handle<Exception>()
               .FallbackAsync((Document)null);

            using (var stream = file.OpenReadStream())
            {
                var document = await documentPolicy
                .ExecuteAsync(async () => await _documentService.SaveDocumentAsync(stream, file.FileName, (int)file.Length, userId))
                .ConfigureAwait(false);

                if (document != null)
                {
                    return Ok(document);
                }
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Deletes the document by specified id.
        /// </summary>
        /// <param name="userId">Id of user that performing operation.</param>
        /// <param name="id">The document id.</param>
        /// <returns>returns Status.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute]string userId, string id)
        {
            var documentPolicy = Policy<bool>
               .Handle<Exception>()
               .FallbackAsync((bool)false);

            var isDeleted = await documentPolicy
                .ExecuteAsync(async () => await _documentService.DeleteDocumentAsync(id, userId))
                .ConfigureAwait(false);
            
            if (!isDeleted)
            {
                var message = $"Document by id: {id} does not exist.";
                _logger.LogTrace(message);

                return NotFound(message);
            }

            return NoContent();
        }
    }
}
