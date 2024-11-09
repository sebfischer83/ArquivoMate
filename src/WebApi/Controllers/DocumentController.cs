﻿using ArquivoMate.Application.Commands.Document;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArquivoMate.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> logger;
        private readonly IMediator mediator;

        public DocumentController(ILogger<DocumentController> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            EnqueueDocumentCommand command = new EnqueueDocumentCommand
            {
                DocumentId = Guid.NewGuid(),
                DocumentName = file.FileName,
                DocumentPath = ""
            };

            await mediator.Send(command);

            return Ok(command.DocumentId);
        }

        [HttpGet("GetStatus")]
        public void GetStatus(Guid id)
        {

        }
    }
}
