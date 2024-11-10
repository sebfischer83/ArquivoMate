using ArquivoMate.Application.Commands.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Interfaces
{
    public interface IDocumentProcessor
    {
        Task ProcessDocument(ProcessDocumentCommand processDocumentCommand);
    }
}
