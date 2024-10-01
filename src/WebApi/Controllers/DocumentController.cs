using ArquivoMate.Application.Document;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ArquivoMate.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> logger;
        private readonly IBus bus;

        public DocumentController(ILogger<DocumentController> logger, IBus bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            await bus.Publish<DocumentAddedMessage>(new DocumentAddedMessage() { FileName = file.FileName });

            return NoContent();
        }

        [HttpGet("GetStatus")]
        public void GetStatus(Guid id)
        {

        }
    }
}
