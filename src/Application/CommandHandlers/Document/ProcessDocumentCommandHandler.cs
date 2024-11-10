using ArquivoMate.Application.Commands.Document;
using ArquivoMate.Application.Interfaces;
using MediatR;

namespace ArquivoMate.Application.CommandHandlers.Document
{
    public class ProcessDocumentCommandHandler : IRequestHandler<ProcessDocumentCommand>
    {
        private readonly IDocumentProcessor documentProcessor;

        public ProcessDocumentCommandHandler(IDocumentProcessor documentProcessor)
        {
            this.documentProcessor = documentProcessor;
        }

        async Task IRequestHandler<ProcessDocumentCommand>.Handle(ProcessDocumentCommand request, CancellationToken cancellationToken)
        {
            await documentProcessor.ProcessDocument(request);
        }
    }
}
