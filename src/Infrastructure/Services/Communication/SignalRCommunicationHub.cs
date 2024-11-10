using ArquivoMate.Application;
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
    public class SignalRCommunicationHub : Hub, ICommunicationHub
    {
        private readonly ILogger<SignalRCommunicationHub> logger;

        public SignalRCommunicationHub(ILogger<SignalRCommunicationHub> logger)
        {
            this.logger = logger;
        }

        public async Task SendDocumentStatus(string connectionId, string documentId, HubResponse<HubResponseProgressData> status)
        {
            await Clients.All.SendAsync("DocumentStatus", documentId, status);
        }
    }
}
