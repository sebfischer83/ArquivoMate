using ArquivoMate.Application;
using ArquivoMate.Application.Interfaces;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Communication
{
    public class SignalRCommunicationHub : Hub
    {
        private readonly ILogger<SignalRCommunicationHub> logger;
    }

    public class SignalRCommunicationService : ICommunicationHub
    {
        private readonly IHubContext<SignalRCommunicationHub> hubContext;

        public SignalRCommunicationService(IHubContext<SignalRCommunicationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task SendDocumentStatus(string connectionId, string documentId, HubResponse<HubResponseProgressData> status)
        {
            await hubContext.Clients.All.SendAsync("DocumentStatus", documentId, status);
        }
    }
}
