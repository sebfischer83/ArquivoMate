using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Shared;
using ArquivoMate.Shared.Document;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArquivoMate.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
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
        [ProducesResponseType(typeof(AppResponse<AddFileResponse>), StatusCodes.Status201Created)]
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

            logger.LogDebug($"Addfile called with {file.FileName} {file.Length} {file.ContentType}");
            var documentId = Guid.NewGuid();
            var tempPath = Path.Combine(Path.GetTempPath(), documentId.ToString());
            Path.ChangeExtension(tempPath, Path.GetExtension(file.FileName));
            System.IO.File.WriteAllBytes(tempPath, fileBytes);
            logger.LogDebug($"Temp file created at {tempPath}");

            EnqueueDocumentCommand command = new EnqueueDocumentCommand
            {
                DocumentId = documentId,
                DocumentName = file.FileName,
                DocumentPath = tempPath
            };

            await mediator.Send(command);

            var result = new AppResponse<AddFileResponse>().SetSuccessResponse(new AddFileResponse { TaskId = command.DocumentId });

            return Ok(result);
        }
    }
}
