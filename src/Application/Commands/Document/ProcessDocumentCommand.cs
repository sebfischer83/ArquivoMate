using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Commands.Document
{
    public class ProcessDocumentCommand : IRequest
    {
        public Guid DocumentId { get; set; }

        public string DocumentName { get; set; }

        public string DocumentPath { get; set; }


        public ProcessDocumentCommand(EnqueueDocumentCommand enqueueDocumentCommand)
        {
            DocumentId = enqueueDocumentCommand.DocumentId;
            DocumentName = enqueueDocumentCommand.DocumentName;
            DocumentPath = enqueueDocumentCommand.DocumentPath;
        }
    }
}
