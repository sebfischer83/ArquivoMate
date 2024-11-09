using ArquivoMate.Application.Commands.Document;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Consumer
{
    internal class EnqueueDocumentConsumer : IConsumer<EnqueueDocumentCommand>
    {
        private readonly IMediator _mediator;

        public EnqueueDocumentConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<EnqueueDocumentCommand> context)
        {
            var command = new ProcessDocumentCommand(context.Message);
            await _mediator.Send(command);
        }
    }
}
