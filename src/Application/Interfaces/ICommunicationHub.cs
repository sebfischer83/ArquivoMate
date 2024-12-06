using ArquivoMate.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Interfaces
{
    public interface ICommunicationHub
    {
        public Task SendDocumentStatus(string connectionId, string documentId, HubResponse<DocumentProcessingFinishedData> status);
    }
}
