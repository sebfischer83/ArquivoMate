using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Document
{
    public class DocumentAddedMessage
    {
        public Guid EventId { get; set; }
        public required string FileName { get; set; }
    }

    public class DocumentAddedConsumer : IConsumer<DocumentAddedMessage>
    {
        private readonly ILogger<DocumentAddedConsumer> logger;

        public DocumentAddedConsumer(ILogger<DocumentAddedConsumer> logger)
        {
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<DocumentAddedMessage> context)
        {
            logger.LogInformation("Test");
            return Task.CompletedTask;
        }
    }
}
