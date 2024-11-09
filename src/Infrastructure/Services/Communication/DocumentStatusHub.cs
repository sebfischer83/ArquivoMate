using ArquivoMate.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Communication
{
    public class DocumentStatusHub : Hub, ICommunicationHub
    {
        private readonly ILogger<DocumentStatusHub> logger;

        public DocumentStatusHub(ILogger<DocumentStatusHub> logger)
        {
            this.logger = logger;
        }

        public async Task SendDocumentStatus(string connectionId, string documentId, string status)
        {
            await Clients.All.SendAsync("DocumentStatus", documentId, status);
        }
    }
}
