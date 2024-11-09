using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArquivoMate.Application.CommandHandlers.Document
{
    public class EnqueueDocumentCommandHandler : IRequestHandler<EnqueueDocumentCommand>
    {
        private readonly IMessageQueue<EnqueueDocumentCommand> _messageQueue;

        public EnqueueDocumentCommandHandler(IMessageQueue<EnqueueDocumentCommand> messageQueue)
        {
            _messageQueue = messageQueue;
        }

        async Task IRequestHandler<EnqueueDocumentCommand>.Handle(EnqueueDocumentCommand request, CancellationToken cancellationToken)
        {
            await _messageQueue.EnqueueMessage(request);
        }
    }
}
